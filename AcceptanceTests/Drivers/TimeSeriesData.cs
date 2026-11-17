namespace NordicRCC.AcceptanceTests.Drivers;

public sealed record TimeSeriesData(
    DateTimeOffset Timestamp,
    string BiddingZone,
    decimal Production,
    decimal Consumption,
    decimal CrossBorderFlow);
