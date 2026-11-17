namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// Checks the rules that a new time series data entry must follow on its own, without looking at the data
/// that the system already holds.
/// </summary>
public static class TimeSeriesDataValidator
{
    /// <summary>
    /// The number of decimal places that the system stores.
    /// </summary>
    public const int MaxDecimalPlaces = 3;

    public static IReadOnlyList<TimeSeriesDataValidationError> Validate(NewTimeSeriesData data)
    {
        var errors = new List<TimeSeriesDataValidationError>();

        if (data.Timestamp is null)
        {
            errors.Add(TimeSeriesDataValidationError.TimestampMissing);
        }

        if (string.IsNullOrWhiteSpace(data.BiddingZone))
        {
            errors.Add(TimeSeriesDataValidationError.BiddingZoneMissing);
        }
        else if (!BiddingZones.Nordic.Contains(data.BiddingZone))
        {
            errors.Add(TimeSeriesDataValidationError.BiddingZoneUnknown);
        }

        switch (data.Production)
        {
            case null:
                errors.Add(TimeSeriesDataValidationError.ProductionMissing);
                break;
            case < 0:
                errors.Add(TimeSeriesDataValidationError.ProductionNegative);
                break;
        }

        if (HasTooManyDecimalPlaces(data.Production))
        {
            errors.Add(TimeSeriesDataValidationError.ProductionTooManyDecimalPlaces);
        }

        switch (data.Consumption)
        {
            case null:
                errors.Add(TimeSeriesDataValidationError.ConsumptionMissing);
                break;
            case < 0:
                errors.Add(TimeSeriesDataValidationError.ConsumptionNegative);
                break;
        }

        if (HasTooManyDecimalPlaces(data.Consumption))
        {
            errors.Add(TimeSeriesDataValidationError.ConsumptionTooManyDecimalPlaces);
        }

        if (data.CrossBorderFlow is null)
        {
            errors.Add(TimeSeriesDataValidationError.CrossBorderFlowMissing);
        }
        else if (HasTooManyDecimalPlaces(data.CrossBorderFlow))
        {
            errors.Add(TimeSeriesDataValidationError.CrossBorderFlowTooManyDecimalPlaces);
        }

        return errors;
    }

    // Trailing zeros don't count, so 3980.2000 has one decimal place.
    private static bool HasTooManyDecimalPlaces(decimal? value) =>
        value is { } number && number != Math.Round(number, MaxDecimalPlaces);
}
