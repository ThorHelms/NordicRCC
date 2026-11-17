namespace NordicRCC.AcceptanceTests.Drivers;

/// <summary>
/// A reason why the system rejected a new time series data entry.
/// </summary>
public enum TimeSeriesDataRejectionReason
{
    TimestampMissing,
    BiddingZoneMissing,
    ProductionMissing,
    ConsumptionMissing,
    CrossBorderFlowMissing,
    BiddingZoneUnknown,
    ProductionNegative,
    ConsumptionNegative,
    ProductionTooManyDecimalPlaces,
    ConsumptionTooManyDecimalPlaces,
    CrossBorderFlowTooManyDecimalPlaces,
    AlreadyExists,
}

/// <summary>
/// The outcome of creating a time series data entry. The entry was accepted when there are no rejection reasons.
/// </summary>
public sealed record TimeSeriesDataCreation(IReadOnlyList<TimeSeriesDataRejectionReason> RejectionReasons)
{
    public static TimeSeriesDataCreation Accepted { get; } = new([]);

    public bool IsAccepted => RejectionReasons.Count == 0;
}
