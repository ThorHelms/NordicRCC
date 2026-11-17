using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NordicRCC.Domain;
using NordicRCC.Infrastructure.Database;

namespace NordicRCC.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    private const string DatabaseName = "NordicRCC";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Each application instance has its own in-memory database, which lives as long as the application.
        services.AddSingleton<InMemoryDatabaseRoot>();
        services.AddDbContext<NordicRccDbContext>((serviceProvider, options) =>
            options.UseInMemoryDatabase(DatabaseName, serviceProvider.GetRequiredService<InMemoryDatabaseRoot>()));
        services.AddScoped<INordicRccDbContext>(serviceProvider =>
            serviceProvider.GetRequiredService<NordicRccDbContext>());

        return services;
    }
}
