using HotChocolate.Demo.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Demo.Data;

/// <summary>
/// EF Core DbContext.
/// Wichtig: Der Kontext wird via IDbContextFactory (pooled) injiziert,
/// nicht direkt als Service. Das ist die empfohlene Praxis fuer
/// GraphQL-Szenarien, da Resolver parallel ausgefuehrt werden koennen.
/// </summary>
public sealed class LibraryDbContext(DbContextOptions<LibraryDbContext> options)
    : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).HasMaxLength(300).IsRequired();
            entity.Property(b => b.Isbn).HasMaxLength(20).IsRequired();
            entity.Property(b => b.Genre).HasConversion<string>();
            entity.HasIndex(b => b.Isbn).IsUnique();
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(a => a.LastName).HasMaxLength(100).IsRequired();
            entity.Ignore(a => a.FullName); // computed
        });
    }
}
