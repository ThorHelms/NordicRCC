using Microsoft.Extensions.DependencyInjection;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ITimeSeriesDataService, TimeSeriesDataService>();

        return services;
    }
}
