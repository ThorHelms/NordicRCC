namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The reasons why a new time series data entry was rejected. There is always at least one.
/// </summary>
public sealed record TimeSeriesDataValidationFailed(IReadOnlyList<TimeSeriesDataValidationError> Errors);

/// <summary>
/// The outcome of creating a time series data entry: either the <see cref="TimeSeriesData"/> that was created,
/// or the <see cref="TimeSeriesDataValidationFailed"/> errors that the entry was rejected for.
/// </summary>
public union CreateTimeSeriesDataResult(TimeSeriesData, TimeSeriesDataValidationFailed);
