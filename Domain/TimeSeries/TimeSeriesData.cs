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
    /// The id that identifies the entry. The system assigns it when the entry is saved, unless the entry
    /// already has an id, for example because it was imported together with its id.
    /// </summary>
    public long Id { get; init; }
}
