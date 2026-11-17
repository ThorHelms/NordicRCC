namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// A time series data entry to create. A value that is <see langword="null"/> is left out of the entry.
/// </summary>
public sealed record NewTimeSeriesData(
    DateTimeOffset? Timestamp,
    string? BiddingZone,
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow);
