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
    /// Gets the time series data entry with the id, or <see cref="TimeSeriesDataNotFound"/> when there is none.
    /// </summary>
    Task<GetTimeSeriesDataByIdResult> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a time series data entry, if it is valid. A rejected entry leaves the data that the system
    /// holds unchanged.
    /// </summary>
    Task<CreateTimeSeriesDataResult> CreateAsync(
        NewTimeSeriesData data,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the production, consumption and cross-border flow of the time series data entry with the id, if the
    /// new values are valid. The timestamp, bidding zone and id of the entry stay as they were. A rejected update
    /// leaves the data that the system holds unchanged.
    /// </summary>
    Task<UpdateTimeSeriesDataResult> UpdateAsync(
        long id,
        UpdatedTimeSeriesData data,
        CancellationToken cancellationToken = default);
}
