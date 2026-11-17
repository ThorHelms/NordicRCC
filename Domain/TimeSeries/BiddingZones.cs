namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The bidding zones that the system holds time series data for.
/// </summary>
public static class BiddingZones
{
    public static IReadOnlySet<string> Nordic { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "NO1", "NO2", "NO3", "NO4", "NO5",
        "SE1", "SE2", "SE3", "SE4",
        "DK1", "DK2",
        "FI",
    };
}
