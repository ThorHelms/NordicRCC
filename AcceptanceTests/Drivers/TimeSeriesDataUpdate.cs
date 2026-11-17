namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// The outcome of updating a time series data entry: either the updated entry, the
/// <see cref="TimeSeriesDataRejected"/> reasons that the update was rejected for, or
/// <see cref="TimeSeriesDataEntryNotFound"/> when the system holds no entry with the id.
/// </summary>
public union TimeSeriesDataUpdate(TimeSeriesData, TimeSeriesDataRejected, TimeSeriesDataEntryNotFound);
