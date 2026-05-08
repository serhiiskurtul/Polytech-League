using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PolytechLeague.Persistence.Database;
using System.IO;

namespace PolytechLeague.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Вказуємо шлях до папки з конфігурацією (API проект)
        string path = Path.Combine(Directory.GetCurrentDirectory(), "..", "PolytechLeague.API");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets("1c37ec15-1dad-47c4-8fdf-57ccd4dcd988")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString, x => x.MigrationsAssembly("PolytechLeague.Persistence"));

        return new ApplicationDbContext(builder.Options);
    }
}
