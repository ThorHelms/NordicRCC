using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

/// <summary>
/// The new values of a time series data entry, read from the request body. The timestamp and the bidding zone
/// can't be updated, so they aren't part of the request. Every value is required, but they are nullable so that a
/// missing value is reported as a validation error rather than as a body that can't be read.
/// </summary>
public sealed record UpdateTimeSeriesDataRequest(
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow)
{
    public UpdatedTimeSeriesData ToUpdatedTimeSeriesData() => new(Production, Consumption, CrossBorderFlow);
}
