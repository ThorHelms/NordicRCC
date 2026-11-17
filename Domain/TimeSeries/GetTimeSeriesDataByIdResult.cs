namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The system holds no time series data entry with the id.
/// </summary>
public sealed record TimeSeriesDataNotFound(long Id);

/// <summary>
/// The outcome of fetching a time series data entry by its id: either the <see cref="TimeSeriesData"/> entry,
/// or <see cref="TimeSeriesDataNotFound"/> when the system holds no entry with the id.
/// </summary>
public union GetTimeSeriesDataByIdResult(TimeSeriesData, TimeSeriesDataNotFound);
