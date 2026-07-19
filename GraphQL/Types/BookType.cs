using HotChocolate.Demo.Domain;
using HotChocolate.Demo.GraphQL.DataLoaders;
using HotChocolate.Types;

namespace HotChocolate.Demo.GraphQL.Types;

/// <summary>
/// Explicit Code-First Definition des GraphQL-Typs "Book".
/// Vorteile gegenueber implizitem Mapping:
///   - volle Kontrolle ueber Nullability
///   - dokumentierte Felder (Description)
///   - stabile Schema-Evolution (Federation, Versionierung)
///   - Resolver fuer abgeleitete Felder (z.B. author via DataLoader)
/// </summary>
public sealed class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        descriptor
            .Description("Ein Buch in der Bibliothek.");

        // Alle Skalar-Felder uebernehmen ...
        descriptor.BindFieldsImplicitly();

        // ... und gezielt ueberschreiben:
        descriptor.Field(b => b.Id)
            .Type<NonNullType<IdType>>()
            .Description("Eindeutige Buch-Id (UUID).");

        descriptor.Field(b => b.Title)
            .Type<NonNullType<StringType>>()
            .Description("Titel des Buches.");

        descriptor.Field(b => b.Isbn)
            .Type<NonNullType<StringType>>()
            .Description("ISBN-13 des Buches.");

        descriptor.Field(b => b.PublishedOn)
            .Type<NonNullType<DateType>>()
            .Description("Veroeffentlichungsdatum.");

        descriptor.Field(b => b.Genre)
            .Type<NonNullType<EnumType<Genre>>>()
            .Description("Genre des Buches.");

        // AuthorId als interne Referenz verstecken, stattdessen
        // ein auflösbares Feld "author" anbieten (Batch via DataLoader).
        descriptor.Ignore(b => b.AuthorId);

        descriptor.Field("author")
            .Type<NonNullType<AuthorType>>()
            .Description("Autor des Buches.")
            .ResolveWith<BookResolvers>(r => r.GetAuthorAsync(default!, default!, default));
    }

    /// <summary>
    /// Resolver-Klasse haelt komplexere Feld-Resolver getrennt vom Type,
    /// laesst sich besser testen und DI-faehig halten.
    /// </summary>
    private sealed class BookResolvers
    {
        public Task<Author> GetAuthorAsync(
            [Parent] Book book,
            AuthorByIdDataLoader loader,
            CancellationToken ct)
            => loader.LoadRequiredAsync(book.AuthorId, ct);
    }
}
