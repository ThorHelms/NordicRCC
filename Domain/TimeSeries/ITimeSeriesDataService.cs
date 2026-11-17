namespace NordicRCC.Domain.TimeSeries;

public interface ITimeSeriesDataService
{
    /// <summary>
    /// Gets the time series data that matches the filter, ordered by timestamp and then by bidding zone.
    /// </summary>
    Task<IReadOnlyList<TimeSeriesData>> GetAsync(
        TimeSeriesDataFilter filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a time series data entry, if it is valid. A rejected entry leaves the data that the system
    /// holds unchanged.
    /// </summary>
    Task<CreateTimeSeriesDataResult> CreateAsync(
        NewTimeSeriesData data,
        CancellationToken cancellationToken = default);
}
