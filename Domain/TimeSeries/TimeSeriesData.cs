namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// The electricity production, consumption and cross-border electricity flow of a bidding zone at a point in time.
/// </summary>
public sealed record TimeSeriesData(
    DateTimeOffset Timestamp,
    string BiddingZone,
    decimal Production,
    decimal Consumption,
    decimal CrossBorderFlow)
{
    /// <summary>
    /// The identity of the data in the database, assigned when it is saved.
    /// </summary>
    public long Id { get; private set; }
}
