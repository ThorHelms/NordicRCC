namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// A reason why a new time series data entry is rejected.
/// </summary>
public enum TimeSeriesDataValidationError
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

    /// <summary>
    /// The system already holds time series data for the bidding zone at the timestamp.
    /// </summary>
    AlreadyExists,
}
