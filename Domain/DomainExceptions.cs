namespace HotChocolate.Demo.Domain;

/// <summary>
/// Basis-Exception fuer Domaenenfehler.
/// Im GraphQL-Layer werden diese vom DomainErrorFilter
/// in strukturierte GraphQL-Errors uebersetzt.
/// </summary>
public abstract class DomainException(string message) : Exception(message);

public sealed class BookNotFoundException(Guid bookId)
    : DomainException($"Buch mit Id '{bookId}' wurde nicht gefunden.")
{
    public Guid BookId { get; } = bookId;
}

public sealed class AuthorNotFoundException(Guid authorId)
    : DomainException($"Autor mit Id '{authorId}' wurde nicht gefunden.")
{
    public Guid AuthorId { get; } = authorId;
}

public sealed class ValidationException(string message) : DomainException(message);
