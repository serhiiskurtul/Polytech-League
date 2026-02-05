using CampusLeague.Persistence.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace CampusLeague.Infrastructure.Persistence.Database;
public static class Persistence
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("CampusLeague.Persistence")
            ));

        return services;
    }
}
