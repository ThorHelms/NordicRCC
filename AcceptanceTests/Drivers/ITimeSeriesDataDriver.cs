namespace NordicRCC.AcceptanceTests.Drivers;

public interface ITimeSeriesDataDriver
{
    /// <summary>
    /// Records time series data in the system, as if it had been received from its source.
    /// </summary>
    Task RecordAsync(IEnumerable<TimeSeriesData> data);

    /// <summary>
    /// Creates a time series data entry through the system, which accepts or rejects it.
    /// </summary>
    Task<TimeSeriesDataCreation> CreateAsync(NewTimeSeriesData data);

    Task<IReadOnlyList<TimeSeriesData>> FetchAllAsync();

    Task<IReadOnlyList<TimeSeriesData>> FetchForBiddingZoneAsync(string biddingZone);

    /// <summary>
    /// Fetches the time series data from <paramref name="from"/> (inclusive) up to <paramref name="to"/> (exclusive).
    /// A point in time that is <see langword="null"/> leaves the period open at that end.
    /// </summary>
    Task<IReadOnlyList<TimeSeriesData>> FetchForPeriodAsync(DateTimeOffset? from, DateTimeOffset? to);
}
