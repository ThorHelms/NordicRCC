using System.Globalization;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using NordicRCC.Domain;
using StoredTimeSeriesData = NordicRCC.Domain.TimeSeries.TimeSeriesData;

namespace NordicRCC.AcceptanceTests.Drivers.Api;

public sealed class ApiTimeSeriesDataDriver(ApiHost host) : ITimeSeriesDataDriver
{
    // Users can't add time series data through the system, so it's stored in the database directly.
    public async Task RecordAsync(IEnumerable<TimeSeriesData> data)
    {
        using var scope = host.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<INordicRccDbContext>();

        database.TimeSeriesData.AddRange(data.Select(item => new StoredTimeSeriesData(
            item.Timestamp, item.BiddingZone, item.Production, item.Consumption, item.CrossBorderFlow)));

        await database.SaveChangesAsync();
    }

    public Task<IReadOnlyList<TimeSeriesData>> FetchAllAsync() => FetchAsync("/time-series-data");

    public Task<IReadOnlyList<TimeSeriesData>> FetchForBiddingZoneAsync(string biddingZone) =>
        FetchAsync($"/time-series-data?biddingZone={Uri.EscapeDataString(biddingZone)}");

    public Task<IReadOnlyList<TimeSeriesData>> FetchForPeriodAsync(DateTimeOffset? from, DateTimeOffset? to)
    {
        var parameters = new List<string>();

        if (from is { } fromValue)
        {
            parameters.Add($"from={Uri.EscapeDataString(fromValue.ToString("O", CultureInfo.InvariantCulture))}");
        }

        if (to is { } toValue)
        {
            parameters.Add($"to={Uri.EscapeDataString(toValue.ToString("O", CultureInfo.InvariantCulture))}");
        }

        return FetchAsync(parameters.Count == 0
            ? "/time-series-data"
            : $"/time-series-data?{string.Join('&', parameters)}");
    }

    private async Task<IReadOnlyList<TimeSeriesData>> FetchAsync(string requestUri)
    {
        var response = await host.Client.GetFromJsonAsync<List<TimeSeriesDataResponse>>(requestUri)
            ?? throw new InvalidOperationException("The system returned no time series data, not even an empty list.");

        return [.. response.Select(item => new TimeSeriesData(
            item.Timestamp, item.BiddingZone, item.Production, item.Consumption, item.CrossBorderFlow))];
    }

    private sealed record TimeSeriesDataResponse(
        DateTimeOffset Timestamp,
        string BiddingZone,
        decimal Production,
        decimal Consumption,
        decimal CrossBorderFlow);
}
