namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// Narrows down which time series data is fetched. A criterion that is <see langword="null"/> doesn't filter.
/// </summary>
/// <param name="BiddingZone">Only data of this bidding zone.</param>
public sealed record TimeSeriesDataFilter(string? BiddingZone = null);
