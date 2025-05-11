using System.Reflection;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<FavoriteComic> FavoriteComics { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = config.GetConnectionString("DataBaseConnection");
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.UseNetTopologySuite();
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}