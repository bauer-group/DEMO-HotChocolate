namespace HotChocolate.Demo.Domain;

/// <summary>
/// Buch-Entitaet der Domaene.
/// Regeln: immutable via init-only Setter, keine Framework-Attribute,
/// keine EF-Core-Abhaengigkeiten.
/// </summary>
public sealed class Book
{
    public Guid Id { get; init; }

    public string Title { get; init; } = default!;

    public string Isbn { get; init; } = default!;

    public DateOnly PublishedOn { get; init; }

    public int PageCount { get; init; }

    public Genre Genre { get; init; }

    // Beziehungen werden als Fremdschluessel ausgedrueckt.
    // Die Navigation uebernimmt der DataLoader im GraphQL-Layer.
    public Guid AuthorId { get; init; }
}

public enum Genre
{
    Unknown = 0,
    Fiction = 1,
    NonFiction = 2,
    ScienceFiction = 3,
    Fantasy = 4,
    Thriller = 5,
    Biography = 6,
    Technical = 7
}
