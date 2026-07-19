using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Demo.Application;
using HotChocolate.Demo.Domain;
using HotChocolate.Types;

namespace HotChocolate.Demo.GraphQL;

/// <summary>
/// Root-Query des GraphQL-Schemas.
/// Resolver sind bewusst duenn - Orchestrierung, keine Business-Logik.
/// </summary>
public sealed class Query
{
    // -------------------------------------------------------------------------
    // Paged / Filtered / Sorted Collection
    // -------------------------------------------------------------------------
    // Reihenfolge der Attribute ist relevant: UsePaging muss zuerst stehen,
    // damit Filtering/Sorting innerhalb der Page-Grenzen angewandt werden.
    // -------------------------------------------------------------------------
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 25, MaxPageSize = 100)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Book> GetBooks([Service] BookService service)
        => service.QueryBooks();

    // -------------------------------------------------------------------------
    // Einzelner Record ueber Id
    // -------------------------------------------------------------------------
    public Task<Book?> GetBookByIdAsync(
        Guid id,
        [Service] BookService service,
        CancellationToken ct)
        => service.GetByIdAsync(id, ct);

    // -------------------------------------------------------------------------
    // Autoren mit geschuetztem Zugriff (Authorization)
    // -------------------------------------------------------------------------
    [Authorize(Policy = "reader")]
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Author> GetAuthors([Service] AuthorService service)
        => service.QueryAuthors();

    public Task<Author?> GetAuthorByIdAsync(
        Guid id,
        [Service] AuthorService service,
        CancellationToken ct)
        => service.GetByIdAsync(id, ct);
}
