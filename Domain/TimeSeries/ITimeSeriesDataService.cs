namespace NordicRCC.Domain.TimeSeries;

public interface ITimeSeriesDataService
{
    /// <summary>
    /// Gets all time series data, ordered by timestamp and then by bidding zone.
    /// </summary>
    Task<IReadOnlyList<TimeSeriesData>> GetAllAsync(CancellationToken cancellationToken = default);
}
