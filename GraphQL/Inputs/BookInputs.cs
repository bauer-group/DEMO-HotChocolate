using HotChocolate.Demo.Domain;

namespace HotChocolate.Demo.GraphQL.Inputs;

/// <summary>
/// Input fuer AddBook-Mutation. Records bevorzugt - immutable,
/// mit struktureller Gleichheit, ohne Boilerplate.
/// </summary>
public sealed record AddBookInput(
    string Title,
    string Isbn,
    DateOnly PublishedOn,
    int PageCount,
    Genre Genre,
    Guid AuthorId);

public sealed record UpdateBookInput(
    Guid Id,
    string? Title = null,
    int? PageCount = null,
    Genre? Genre = null);

public sealed record DeleteBookInput(Guid Id);

public sealed record AddAuthorInput(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string? Biography = null);
