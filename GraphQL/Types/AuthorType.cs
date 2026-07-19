using HotChocolate.Demo.Domain;
using HotChocolate.Demo.GraphQL.DataLoaders;
using HotChocolate.Types;

namespace HotChocolate.Demo.GraphQL.Types;

public sealed class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        descriptor.Description("Ein Autor.");

        descriptor.BindFieldsImplicitly();

        descriptor.Field(a => a.Id).Type<NonNullType<IdType>>();
        descriptor.Field(a => a.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.LastName).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.FullName).Type<NonNullType<StringType>>();
        descriptor.Field(a => a.BirthDate).Type<NonNullType<DateType>>();

        // Liste aller Buecher dieses Autors - via DataLoader
        descriptor.Field("books")
            .Type<NonNullType<ListType<NonNullType<BookType>>>>()
            .Description("Alle Buecher dieses Autors.")
            .ResolveWith<AuthorResolvers>(r => r.GetBooksAsync(default!, default!, default));
    }

    private sealed class AuthorResolvers
    {
        public async Task<Book[]> GetBooksAsync(
            [Parent] Author author,
            BooksByAuthorIdDataLoader loader,
            CancellationToken ct)
        {
            var books = await loader.LoadAsync(author.Id, ct);
            return books ?? [];
        }
    }
}
