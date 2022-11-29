using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Reporting.Api.Infrastructure.EntityConfigurations;

namespace Reporting.Api.Infrastructure;

/// <summary>
/// Represents the database context that operates on the phone book reporting related entities.
/// </summary>
public class ReportingContext : DbContext
{
    /// <summary>
    /// Represents the reports table.
    /// </summary>
    public DbSet<Models.Report> Reports { get; set; }

    /// <summary>
    /// Creates an instance that can be used to access the reporting database.
    /// </summary>
    /// <param name="options">The options to further configure the database context.</param>
    public ReportingContext(DbContextOptions<ReportingContext> options) : base(options) { }

    /// <summary>
    /// Applies any configuration procedures while creating the models on the database.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure models on the database.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReportEntityTypeConfiguration());
    }
}

/// <summary>
/// Contains methods to configure and create database contexts.
/// </summary>
// ReSharper disable once UnusedMember.Global (Used by Entity Framework CLI Tools for migrations)
public class ReportingContextDesignFactory : IDesignTimeDbContextFactory<ReportingContext>
{
    /// <summary>
    /// Creates a <see cref="ReportingContext"/> with the default connection string. Typically used by Entity Framework CLI Tools for migrations.
    /// </summary>
    /// <param name="args">The arguments to be used in the creation.</param>
    /// <returns>The newly created <see cref="ReportingContext"/>.</returns>
    /// <remarks>This must be as the same connection string as the one in <c>appsettings.json</c>.</remarks>
    public ReportingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReportingContext>()
            .UseNpgsql("Host=localhost; Port=5432; Username=reportingapiuser; Password=Vn0th3rS7rangePa5sworD; Database=phonebookreporting;");

        return new ReportingContext(optionsBuilder.Options);
    }
}