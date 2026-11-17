using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

public sealed record TimeSeriesDataDto(
    long Id,
    DateTimeOffset Timestamp,
    string BiddingZone,
    decimal Production,
    decimal Consumption,
    decimal CrossBorderFlow)
{
    public static TimeSeriesDataDto From(TimeSeriesData data) =>
        new(data.Id, data.Timestamp, data.BiddingZone, data.Production, data.Consumption, data.CrossBorderFlow);
}
