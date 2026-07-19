using GreenDonut;
using HotChocolate.Demo.Application;
using HotChocolate.Demo.Domain;

namespace HotChocolate.Demo.GraphQL.DataLoaders;

/// <summary>
/// Batch-DataLoader fuer Autoren.
/// Zweck: wenn in einer Query 100 Buecher ihren Autor aufloesen wollen,
/// werden statt 100 Einzelqueries alle Ids in einem Batch gesammelt und
/// mit einer einzigen Datenbank-Abfrage aufgeloest.
/// HotChocolate startet pro Request einen eigenen Loader-Scope inklusive
/// Request-lokalem Cache.
/// </summary>
public sealed class AuthorByIdDataLoader(
    AuthorService authorService,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options)
    : BatchDataLoader<Guid, Author>(batchScheduler, options)
{
    protected override Task<IReadOnlyDictionary<Guid, Author>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken ct)
        => authorService.GetByIdsAsync(keys, ct);
}

/// <summary>
/// GroupedDataLoader: liefert pro Autor-Id eine Liste von Buechern.
/// </summary>
public sealed class BooksByAuthorIdDataLoader(
    AuthorService authorService,
    IBatchScheduler batchScheduler,
    DataLoaderOptions options)
    : GroupedDataLoader<Guid, Book>(batchScheduler, options)
{
    protected override async Task<ILookup<Guid, Book>> LoadGroupedBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken ct)
    {
        var dict = await authorService.GetBooksByAuthorIdsAsync(keys, ct);

        // alle Books flat in ein Lookup umwandeln
        return dict
            .SelectMany(kvp => kvp.Value.Select(book => (kvp.Key, book)))
            .ToLookup(x => x.Key, x => x.book);
    }
}
