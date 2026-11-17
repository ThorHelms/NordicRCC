namespace NordicRCC.Domain.TimeSeries;

public interface ITimeSeriesDataService
{
    /// <summary>
    /// Gets the time series data that matches the filter, ordered by timestamp and then by bidding zone.
    /// </summary>
    Task<IReadOnlyList<TimeSeriesData>> GetAsync(
        TimeSeriesDataFilter filter,
        CancellationToken cancellationToken = default);
}
