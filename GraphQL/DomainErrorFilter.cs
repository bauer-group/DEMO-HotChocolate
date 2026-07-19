using HotChocolate.Demo.Domain;

namespace HotChocolate.Demo.GraphQL;

/// <summary>
/// Mappt Domaenen-Exceptions auf strukturierte GraphQL-Errors
/// mit stabilem Error-Code.
/// Vorteil: Clients koennen programmatisch reagieren (switch auf code),
/// statt Exception-Texte parsen zu muessen.
/// </summary>
public sealed class DomainErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        return error.Exception switch
        {
            BookNotFoundException ex => error
                .WithMessage(ex.Message)
                .WithCode("BOOK_NOT_FOUND")
                .SetExtension("bookId", ex.BookId),

            AuthorNotFoundException ex => error
                .WithMessage(ex.Message)
                .WithCode("AUTHOR_NOT_FOUND")
                .SetExtension("authorId", ex.AuthorId),

            ValidationException ex => error
                .WithMessage(ex.Message)
                .WithCode("VALIDATION_FAILED"),

            _ => error // Unbekannte Fehler unveraendert weiterreichen
        };
    }
}
