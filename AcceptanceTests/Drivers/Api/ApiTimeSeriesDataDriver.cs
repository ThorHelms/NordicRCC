using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using NordicRCC.Domain;
using StoredTimeSeriesData = NordicRCC.Domain.TimeSeries.TimeSeriesData;

namespace NordicRCC.AcceptanceTests.Drivers.Api;

public sealed class ApiTimeSeriesDataDriver(ApiHost host) : ITimeSeriesDataDriver
{
    // Recorded data comes from an external source, not from users, so it's stored in the database directly,
    // together with its id.
    public async Task RecordAsync(IEnumerable<TimeSeriesData> data)
    {
        using var scope = host.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<INordicRccDbContext>();

        database.TimeSeriesData.AddRange(data.Select(item => new StoredTimeSeriesData(
            item.Timestamp, item.BiddingZone, item.Production, item.Consumption, item.CrossBorderFlow)
        {
            Id = item.Id,
        }));

        await database.SaveChangesAsync();
    }

    public async Task<TimeSeriesDataCreation> CreateAsync(NewTimeSeriesData data)
    {
        using var response = await host.Client.PostAsJsonAsync("/time-series-data", data);

        switch (response.StatusCode)
        {
            case HttpStatusCode.Created:
                var created = await response.Content.ReadFromJsonAsync<TimeSeriesDataResponse>()
                    ?? throw new InvalidOperationException(
                        "The system accepted the time series data without returning the created entry.");

                return new TimeSeriesDataAccepted(created.Id);

            case HttpStatusCode.BadRequest or HttpStatusCode.Conflict:
                return await ReadRejectionAsync(response);

            default:
                throw UnexpectedResponse("the new time series data", response, await response.Content.ReadAsStringAsync());
        }
    }

    public async Task<TimeSeriesDataUpdate> UpdateAsync(long id, UpdatedTimeSeriesData data)
    {
        using var response = await host.Client.PutAsJsonAsync(
            $"/time-series-data/{id.ToString(CultureInfo.InvariantCulture)}", data);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var updated = await response.Content.ReadFromJsonAsync<TimeSeriesDataResponse>()
                    ?? throw new InvalidOperationException(
                        "The system accepted the update without returning the updated entry.");

                return updated.ToTimeSeriesData();

            case HttpStatusCode.BadRequest:
                return await ReadRejectionAsync(response);

            case HttpStatusCode.NotFound:
                return new TimeSeriesDataEntryNotFound(id);

            default:
                throw UnexpectedResponse(
                    $"the update of the time series data entry with id {id}",
                    response,
                    await response.Content.ReadAsStringAsync());
        }
    }

    private static async Task<TimeSeriesDataRejected> ReadRejectionAsync(HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();

        if (problem?.ErrorCodes is not { Count: > 0 } errorCodes)
        {
            throw new InvalidOperationException(
                $"The system rejected the time series data without saying why: " +
                $"{await response.Content.ReadAsStringAsync()}");
        }

        return new TimeSeriesDataRejected([.. errorCodes.Select(ToRejectionReason)]);
    }

    public async Task<TimeSeriesDataEntryFetch> FetchByIdAsync(long id)
    {
        using var response = await host.Client.GetAsync($"/time-series-data/{id.ToString(CultureInfo.InvariantCulture)}");

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                var entry = await response.Content.ReadFromJsonAsync<TimeSeriesDataResponse>()
                    ?? throw new InvalidOperationException("The system found the time series data entry, but returned nothing.");

                return entry.ToTimeSeriesData();

            case HttpStatusCode.NotFound:
                return new TimeSeriesDataEntryNotFound(id);

            default:
                throw UnexpectedResponse($"the time series data entry with id {id}", response, await response.Content.ReadAsStringAsync());
        }
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

        return [.. response.Select(item => item.ToTimeSeriesData())];
    }

    private static InvalidOperationException UnexpectedResponse(string subject, HttpResponseMessage response, string body) =>
        new($"The system responded to {subject} with {(int)response.StatusCode} {response.StatusCode}: {body}");

    private static TimeSeriesDataRejectionReason ToRejectionReason(string errorCode) => errorCode switch
    {
        "TimestampMissing" => TimeSeriesDataRejectionReason.TimestampMissing,
        "BiddingZoneMissing" => TimeSeriesDataRejectionReason.BiddingZoneMissing,
        "ProductionMissing" => TimeSeriesDataRejectionReason.ProductionMissing,
        "ConsumptionMissing" => TimeSeriesDataRejectionReason.ConsumptionMissing,
        "CrossBorderFlowMissing" => TimeSeriesDataRejectionReason.CrossBorderFlowMissing,
        "BiddingZoneUnknown" => TimeSeriesDataRejectionReason.BiddingZoneUnknown,
        "ProductionNegative" => TimeSeriesDataRejectionReason.ProductionNegative,
        "ConsumptionNegative" => TimeSeriesDataRejectionReason.ConsumptionNegative,
        "ProductionTooManyDecimalPlaces" => TimeSeriesDataRejectionReason.ProductionTooManyDecimalPlaces,
        "ConsumptionTooManyDecimalPlaces" => TimeSeriesDataRejectionReason.ConsumptionTooManyDecimalPlaces,
        "CrossBorderFlowTooManyDecimalPlaces" => TimeSeriesDataRejectionReason.CrossBorderFlowTooManyDecimalPlaces,
        "AlreadyExists" => TimeSeriesDataRejectionReason.AlreadyExists,
        _ => throw new InvalidOperationException($"The system rejected the time series data for an unknown reason: {errorCode}."),
    };

    private sealed record ProblemResponse(List<string>? ErrorCodes);

    private sealed record TimeSeriesDataResponse(
        long Id,
        DateTimeOffset Timestamp,
        string BiddingZone,
        decimal Production,
        decimal Consumption,
        decimal CrossBorderFlow)
    {
        public TimeSeriesData ToTimeSeriesData() =>
            new(Id, Timestamp, BiddingZone, Production, Consumption, CrossBorderFlow);
    }
}
