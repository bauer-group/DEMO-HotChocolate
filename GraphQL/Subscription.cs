using HotChocolate.Demo.Domain;
using HotChocolate.Types;

namespace HotChocolate.Demo.GraphQL;

/// <summary>
/// Root-Subscription - Clients koennen per WebSocket/SSE auf Events hoeren.
/// HotChocolate leitet die Topic-Events via ITopicEventSender/Receiver weiter.
/// </summary>
public sealed class Subscription
{
    /// <summary>
    /// Feuert, sobald ein neues Buch angelegt wird.
    /// </summary>
    [Subscribe]
    [Topic]
    public Book OnBookAdded([EventMessage] Book book) => book;
}
