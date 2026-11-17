namespace NordicRCC.AcceptanceTests.Drivers;

public interface ITimeSeriesDataDriver
{
    /// <summary>
    /// Records time series data in the system, as if it had been received from its source.
    /// </summary>
    Task RecordAsync(IEnumerable<TimeSeriesData> data);

    Task<IReadOnlyList<TimeSeriesData>> FetchAllAsync();
}
