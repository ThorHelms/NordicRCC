using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

public static class TimeSeriesDataEndpoints
{
    public static IEndpointRouteBuilder MapTimeSeriesDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/time-series-data", GetAllTimeSeriesDataAsync)
            .WithName("GetAllTimeSeriesData");

        return endpoints;
    }

    private static async Task<IReadOnlyList<TimeSeriesDataDto>> GetAllTimeSeriesDataAsync(
        ITimeSeriesDataService timeSeriesData,
        CancellationToken cancellationToken)
    {
        var data = await timeSeriesData.GetAllAsync(cancellationToken);

        return [.. data.Select(TimeSeriesDataDto.From)];
    }
}
