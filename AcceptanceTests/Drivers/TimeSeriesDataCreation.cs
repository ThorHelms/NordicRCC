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
/// The system accepted the new time series data entry, and gave it the id <paramref name="Id"/>.
/// </summary>
public sealed record TimeSeriesDataAccepted(long Id);

/// <summary>
/// The system rejected the new time series data entry. There is always at least one reason.
/// </summary>
public sealed record TimeSeriesDataRejected(IReadOnlyList<TimeSeriesDataRejectionReason> Reasons);

/// <summary>
/// The outcome of creating a time series data entry.
/// </summary>
public union TimeSeriesDataCreation(TimeSeriesDataAccepted, TimeSeriesDataRejected);
