using HotChocolate.Demo.Domain;

namespace HotChocolate.Demo.GraphQL.Payloads;

/// <summary>
/// Payload-Objekte kapseln das Ergebnis einer Mutation. So koennen wir
/// spaeter ohne Breaking Change weitere Felder ergaenzen (z.B. Errors,
/// Timestamps, ClientMutationId).
/// </summary>
public sealed record AddBookPayload(Book Book);

public sealed record UpdateBookPayload(Book Book);

public sealed record DeleteBookPayload(Guid DeletedBookId);

public sealed record AddAuthorPayload(Author Author);
