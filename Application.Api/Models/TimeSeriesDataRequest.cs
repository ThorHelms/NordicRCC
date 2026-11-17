using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

/// <summary>
/// The optional criteria for fetching time series data, read from the query string.
/// </summary>
/// <param name="BiddingZone">When given, only the data of this bidding zone is returned.</param>
public sealed record TimeSeriesDataRequest(string? BiddingZone = null)
{
    public TimeSeriesDataFilter ToFilter() =>
        new(BiddingZone: string.IsNullOrWhiteSpace(BiddingZone) ? null : BiddingZone);
}
