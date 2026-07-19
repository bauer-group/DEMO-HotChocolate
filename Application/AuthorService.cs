using HotChocolate.Demo.Data;
using HotChocolate.Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Demo.Application;

/// <summary>
/// Application Service fuer Autoren.
/// </summary>
public sealed class AuthorService(IDbContextFactory<LibraryDbContext> dbFactory)
{
    public IQueryable<Author> QueryAuthors()
    {
        var db = dbFactory.CreateDbContext();
        return db.Authors.AsNoTracking();
    }

    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    /// <summary>
    /// Batch-Variante fuer DataLoader: laedt mehrere Autoren in einer Query.
    /// </summary>
    public async Task<IReadOnlyDictionary<Guid, Author>> GetByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Authors
            .AsNoTracking()
            .Where(a => ids.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, ct);
    }

    public async Task<IReadOnlyDictionary<Guid, Book[]>> GetBooksByAuthorIdsAsync(
        IReadOnlyList<Guid> authorIds,
        CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var books = await db.Books
            .AsNoTracking()
            .Where(b => authorIds.Contains(b.AuthorId))
            .ToListAsync(ct);

        return books
            .GroupBy(b => b.AuthorId)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }

    public async Task<Author> AddAsync(
        string firstName,
        string lastName,
        DateOnly birthDate,
        string? biography,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("Vor- und Nachname duerfen nicht leer sein.");

        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var author = new Author
        {
            Id = Guid.NewGuid(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            BirthDate = birthDate,
            Biography = biography?.Trim()
        };

        db.Authors.Add(author);
        await db.SaveChangesAsync(ct);
        return author;
    }
}
