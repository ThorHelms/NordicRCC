using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

/// <summary>
/// The optional criteria for fetching time series data, read from the query string.
/// </summary>
/// <param name="BiddingZone">When given, only the data of this bidding zone is returned.</param>
/// <param name="From">When given, only the data at or after this point in time (inclusive) is returned.</param>
/// <param name="To">When given, only the data before this point in time (exclusive) is returned.</param>
public sealed record TimeSeriesDataRequest(
    string? BiddingZone = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null)
{
    public TimeSeriesDataFilter ToFilter() =>
        new(
            BiddingZone: string.IsNullOrWhiteSpace(BiddingZone) ? null : BiddingZone,
            From: From,
            To: To);
}
