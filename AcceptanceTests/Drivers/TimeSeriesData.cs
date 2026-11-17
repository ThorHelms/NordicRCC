namespace NordicRCC.AcceptanceTests.Drivers;

public sealed record TimeSeriesData(
    long Id,
    DateTimeOffset Timestamp,
    string BiddingZone,
    decimal Production,
    decimal Consumption,
    decimal CrossBorderFlow);
