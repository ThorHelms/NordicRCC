namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// Narrows down which time series data is fetched. A criterion that is <see langword="null"/> doesn't filter.
/// </summary>
/// <param name="BiddingZone">Only data of this bidding zone.</param>
/// <param name="From">Only data at or after this point in time (inclusive).</param>
/// <param name="To">Only data before this point in time (exclusive), so that consecutive periods never overlap.</param>
public sealed record TimeSeriesDataFilter(
    string? BiddingZone = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null);
