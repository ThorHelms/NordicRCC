namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The new values that someone wants to give an existing time series data entry. The timestamp and the bidding
/// zone identify what the entry describes, so they can't be updated. Any value may be missing, which makes the
/// update invalid.
/// </summary>
public sealed record UpdatedTimeSeriesData(
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow);
