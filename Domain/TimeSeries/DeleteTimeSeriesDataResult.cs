namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The time series data entry with the id was deleted.
/// </summary>
public sealed record TimeSeriesDataDeleted(long Id);

/// <summary>
/// The outcome of deleting a time series data entry: either <see cref="TimeSeriesDataDeleted"/>, or
/// <see cref="TimeSeriesDataNotFound"/> when the system holds no entry with the id.
/// </summary>
public union DeleteTimeSeriesDataResult(TimeSeriesDataDeleted, TimeSeriesDataNotFound);
