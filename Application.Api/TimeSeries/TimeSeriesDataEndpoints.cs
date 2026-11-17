using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

public static class TimeSeriesDataEndpoints
{
    public static IEndpointRouteBuilder MapTimeSeriesDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/time-series-data", GetTimeSeriesDataAsync)
            .WithName("GetTimeSeriesData");

        return endpoints;
    }

    private static async Task<IReadOnlyList<TimeSeriesDataDto>> GetTimeSeriesDataAsync(
        [AsParameters] TimeSeriesDataRequest request,
        ITimeSeriesDataService timeSeriesData,
        CancellationToken cancellationToken)
    {
        var data = await timeSeriesData.GetAsync(request.ToFilter(), cancellationToken);

        return [.. data.Select(TimeSeriesDataDto.From)];
    }
}
