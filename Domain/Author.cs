namespace HotChocolate.Demo.Domain;

/// <summary>
/// Autor-Entitaet der Domaene.
/// Bewusst ohne Navigation-Property auf Book - die Verknuepfung
/// erfolgt ueber einen DataLoader im GraphQL-Layer (Batching & Caching).
/// </summary>
public sealed class Author
{
    public Guid Id { get; init; }

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;

    public DateOnly BirthDate { get; init; }

    public string? Biography { get; init; }

    public string FullName => $"{FirstName} {LastName}";
}
