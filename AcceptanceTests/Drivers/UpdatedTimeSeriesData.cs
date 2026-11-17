namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// The new values of a time series data entry. A value that is <see langword="null"/> is left out of the update.
/// </summary>
public sealed record UpdatedTimeSeriesData(
    decimal? Production,
    decimal? Consumption,
    decimal? CrossBorderFlow);
