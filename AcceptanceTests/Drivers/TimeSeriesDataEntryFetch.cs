namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// The system holds no time series data entry with the id.
/// </summary>
public sealed record TimeSeriesDataEntryNotFound(long Id);

/// <summary>
/// The outcome of fetching a time series data entry by its id: either the entry, or
/// <see cref="TimeSeriesDataEntryNotFound"/>.
/// </summary>
public union TimeSeriesDataEntryFetch(TimeSeriesData, TimeSeriesDataEntryNotFound);
