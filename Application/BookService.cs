using HotChocolate.Demo.Data;
using HotChocolate.Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Demo.Application;

/// <summary>
/// Application Service fuer Buecher.
/// Enthaelt die gesamte Geschaeftslogik und ist framework-unabhaengig
/// (kein GraphQL-Typ, kein HTTP-Kontext).
/// Kann identisch aus REST-Controllern, Workern oder Tests genutzt werden.
/// </summary>
public sealed class BookService(IDbContextFactory<LibraryDbContext> dbFactory)
{
    /// <summary>
    /// Gibt IQueryable zurueck, damit HotChocolate Paging, Filtering und
    /// Sorting direkt auf SQL-Ebene (bzw. hier In-Memory) ausfuehren kann.
    /// </summary>
    public IQueryable<Book> QueryBooks()
    {
        // Achtung: Der DbContext aus der Factory wird bei IQueryable-Nutzung
        // durch HotChocolate (AddProjections) verwaltet. Fuer einfache Faelle
        // kann man wie hier einen Kontext erstellen.
        var db = dbFactory.CreateDbContext();
        return db.Books.AsNoTracking();
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<Book> AddAsync(
        string title,
        string isbn,
        DateOnly publishedOn,
        int pageCount,
        Genre genre,
        Guid authorId,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        // --- Validierung (hier vereinfacht) -------------------------------
        if (string.IsNullOrWhiteSpace(title))
            throw new ValidationException("Titel darf nicht leer sein.");

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ValidationException("ISBN darf nicht leer sein.");

        if (pageCount <= 0)
            throw new ValidationException("Seitenzahl muss positiv sein.");

        var authorExists = await db.Authors.AnyAsync(a => a.Id == authorId, ct);
        if (!authorExists)
            throw new AuthorNotFoundException(authorId);

        var isbnUsed = await db.Books.AnyAsync(b => b.Isbn == isbn, ct);
        if (isbnUsed)
            throw new ValidationException($"ISBN '{isbn}' ist bereits vergeben.");

        // --- Persistieren -------------------------------------------------
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Isbn = isbn.Trim(),
            PublishedOn = publishedOn,
            PageCount = pageCount,
            Genre = genre,
            AuthorId = authorId
        };

        db.Books.Add(book);
        await db.SaveChangesAsync(ct);
        return book;
    }

    public async Task<Book> UpdateAsync(
        Guid id,
        string? title,
        int? pageCount,
        Genre? genre,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var existing = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct)
                       ?? throw new BookNotFoundException(id);

        // Da Book immutable ist, ersetzen wir die Instanz.
        var updated = new Book
        {
            Id = existing.Id,
            Isbn = existing.Isbn,
            AuthorId = existing.AuthorId,
            PublishedOn = existing.PublishedOn,
            Title = title?.Trim() ?? existing.Title,
            PageCount = pageCount ?? existing.PageCount,
            Genre = genre ?? existing.Genre
        };

        db.Entry(existing).CurrentValues.SetValues(updated);
        await db.SaveChangesAsync(ct);
        return updated;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var existing = await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct)
                       ?? throw new BookNotFoundException(id);

        db.Books.Remove(existing);
        await db.SaveChangesAsync(ct);
    }
}
