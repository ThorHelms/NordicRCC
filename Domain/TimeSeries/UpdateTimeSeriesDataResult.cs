namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The outcome of updating a time series data entry: either the updated <see cref="TimeSeriesData"/>, the
/// <see cref="TimeSeriesDataValidationFailed"/> errors that the update was rejected for, or
/// <see cref="TimeSeriesDataNotFound"/> when the system holds no entry with the id.
/// </summary>
public union UpdateTimeSeriesDataResult(TimeSeriesData, TimeSeriesDataValidationFailed, TimeSeriesDataNotFound);
