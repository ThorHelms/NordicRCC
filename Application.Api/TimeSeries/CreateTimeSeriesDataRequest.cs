using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

/// <summary>
/// A new time series data entry, read from the request body. Every value is required, but they are nullable so
/// that a missing value is reported as a validation error rather than as a body that can't be read.
/// </summary>
public sealed record CreateTimeSeriesDataRequest(
    DateTimeOffset? Timestamp,
    string? BiddingZone,
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow)
{
    public NewTimeSeriesData ToNewTimeSeriesData() =>
        new(Timestamp, BiddingZone, Production, Consumption, CrossBorderFlow);
}
