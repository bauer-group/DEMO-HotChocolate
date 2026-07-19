using HotChocolate.Authorization;
using HotChocolate.Demo.Application;
using HotChocolate.Demo.Domain;
using HotChocolate.Demo.GraphQL.Inputs;
using HotChocolate.Demo.GraphQL.Payloads;
using HotChocolate.Subscriptions;

namespace HotChocolate.Demo.GraphQL;

/// <summary>
/// Root-Mutation des GraphQL-Schemas.
/// Jede Mutation akzeptiert genau ein Input-Objekt und gibt ein
/// strukturiertes Payload zurueck (Relay-Konvention).
/// </summary>
public sealed class Mutation
{
    // -------------------------------------------------------------------------
    // Buch anlegen
    // -------------------------------------------------------------------------
    [Authorize(Policy = "librarian")]
    public async Task<AddBookPayload> AddBookAsync(
        AddBookInput input,
        [Service] BookService service,
        [Service] ITopicEventSender eventSender,
        CancellationToken ct)
    {
        var book = await service.AddAsync(
            input.Title,
            input.Isbn,
            input.PublishedOn,
            input.PageCount,
            input.Genre,
            input.AuthorId,
            ct);

        // Subscription-Event feuern
        await eventSender.SendAsync(nameof(Subscription.OnBookAdded), book, ct);

        return new AddBookPayload(book);
    }

    // -------------------------------------------------------------------------
    // Buch aktualisieren
    // -------------------------------------------------------------------------
    [Authorize(Policy = "librarian")]
    public async Task<UpdateBookPayload> UpdateBookAsync(
        UpdateBookInput input,
        [Service] BookService service,
        CancellationToken ct)
    {
        var book = await service.UpdateAsync(
            input.Id,
            input.Title,
            input.PageCount,
            input.Genre,
            ct);

        return new UpdateBookPayload(book);
    }

    // -------------------------------------------------------------------------
    // Buch loeschen
    // -------------------------------------------------------------------------
    [Authorize(Policy = "librarian")]
    public async Task<DeleteBookPayload> DeleteBookAsync(
        DeleteBookInput input,
        [Service] BookService service,
        CancellationToken ct)
    {
        await service.DeleteAsync(input.Id, ct);
        return new DeleteBookPayload(input.Id);
    }

    // -------------------------------------------------------------------------
    // Autor anlegen
    // -------------------------------------------------------------------------
    [Authorize(Policy = "librarian")]
    public async Task<AddAuthorPayload> AddAuthorAsync(
        AddAuthorInput input,
        [Service] AuthorService service,
        CancellationToken ct)
    {
        var author = await service.AddAsync(
            input.FirstName,
            input.LastName,
            input.BirthDate,
            input.Biography,
            ct);

        return new AddAuthorPayload(author);
    }
}
