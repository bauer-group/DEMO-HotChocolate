using HotChocolate.Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Demo.Data;

/// <summary>
/// Befuellt die In-Memory-Datenbank beim Start mit Demo-Daten.
/// Fuer eine echte Anwendung wuerde hier EF Core Migrations +
/// dediziertes Seed-Skript stehen.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var factory = services.GetRequiredService<IDbContextFactory<LibraryDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        if (await db.Authors.AnyAsync())
        {
            return; // bereits befuellt
        }

        var tolkien = new Author
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            FirstName = "John Ronald Reuel",
            LastName = "Tolkien",
            BirthDate = new DateOnly(1892, 1, 3),
            Biography = "Englischer Philologe und Autor."
        };

        var martin = new Author
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            FirstName = "George R. R.",
            LastName = "Martin",
            BirthDate = new DateOnly(1948, 9, 20),
            Biography = "US-amerikanischer Fantasy-Autor."
        };

        db.Authors.AddRange(tolkien, martin);

        db.Books.AddRange(
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Der Hobbit",
                Isbn = "978-3-608-93818-4",
                PublishedOn = new DateOnly(1937, 9, 21),
                PageCount = 310,
                Genre = Genre.Fantasy,
                AuthorId = tolkien.Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "Der Herr der Ringe",
                Isbn = "978-3-608-93828-3",
                PublishedOn = new DateOnly(1954, 7, 29),
                PageCount = 1216,
                Genre = Genre.Fantasy,
                AuthorId = tolkien.Id
            },
            new Book
            {
                Id = Guid.NewGuid(),
                Title = "A Game of Thrones",
                Isbn = "978-0-553-10354-0",
                PublishedOn = new DateOnly(1996, 8, 1),
                PageCount = 694,
                Genre = Genre.Fantasy,
                AuthorId = martin.Id
            }
        );

        await db.SaveChangesAsync();
    }
}
