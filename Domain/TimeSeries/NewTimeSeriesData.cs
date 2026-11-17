namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// A time series data entry that someone wants to create. Any value may be missing, which makes the entry invalid.
/// </summary>
public sealed record NewTimeSeriesData(
    DateTimeOffset? Timestamp,
    string? BiddingZone,
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow);
