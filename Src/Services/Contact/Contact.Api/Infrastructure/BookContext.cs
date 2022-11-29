using Contact.Api.Infrastructure.EntityConfigurations;
using Contact.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Contact.Api.Infrastructure;

/// <summary>
/// Represents the database context that operates on the phone book contact related entities.
/// </summary>
public class BookContext : DbContext
{
    /// <summary>
    /// Represents the contacts table.
    /// </summary>
    public DbSet<Models.Contact> Contacts { get; set; }

    /// <summary>
    /// Represents the contact information table.
    /// </summary>
    public DbSet<ContactInformation> ContactInformation { get; set; }

    /// <summary>
    /// Creates an instance that can be used to access the phone book database.
    /// </summary>
    /// <param name="options">The options to further configure the database context.</param>
    public BookContext(DbContextOptions<BookContext> options) : base(options) { }

    /// <summary>
    /// Applies any configuration procedures while creating the models on the database.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure models on the database.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ContactInformationEntityTypeConfiguration());
    }
}

/// <summary>
/// Contains methods to configure and create database contexts.
/// </summary>
// ReSharper disable once UnusedMember.Global (Used by Entity Framework CLI Tools for migrations)
public class BookContextDesignFactory : IDesignTimeDbContextFactory<BookContext>
{
    /// <summary>
    /// Creates a <see cref="BookContext"/> with the default connection string. Typically used by Entity Framework CLI Tools for migrations.
    /// </summary>
    /// <param name="args">The arguments to be used in the creation.</param>
    /// <returns>The newly created <see cref="BookContext"/>.</returns>
    /// <remarks>This must be as the same connection string as the one in <c>appsettings.json</c>.</remarks>
    public BookContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookContext>()
            .UseNpgsql("Host=localhost; Port=5432; Username=contactapiuser; Password=50m3$7rangePas5word; Database=phonebookcontact;");

        return new BookContext(optionsBuilder.Options);
    }
}