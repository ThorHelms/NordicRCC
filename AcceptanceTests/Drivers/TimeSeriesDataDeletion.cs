namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// The system deleted the time series data entry with the id.
/// </summary>
public sealed record TimeSeriesDataDeleted(long Id);

/// <summary>
/// The outcome of deleting a time series data entry: either <see cref="TimeSeriesDataDeleted"/>, or
/// <see cref="TimeSeriesDataEntryNotFound"/> when the system holds no entry with the id.
/// </summary>
public union TimeSeriesDataDeletion(TimeSeriesDataDeleted, TimeSeriesDataEntryNotFound);
