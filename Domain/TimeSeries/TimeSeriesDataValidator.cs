namespace NordicRCC.Domain.TimeSeries;

/// <summary>
/// Checks the rules that a new time series data entry, or an update of one, must follow on its own, without
/// looking at the data that the system already holds.
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

        AddValueErrors(errors, data.Production, data.Consumption, data.CrossBorderFlow);

        return errors;
    }

    public static IReadOnlyList<TimeSeriesDataValidationError> Validate(UpdatedTimeSeriesData data)
    {
        var errors = new List<TimeSeriesDataValidationError>();

        AddValueErrors(errors, data.Production, data.Consumption, data.CrossBorderFlow);

        return errors;
    }

    private static void AddValueErrors(
        List<TimeSeriesDataValidationError> errors,
        decimal? production,
        decimal? consumption,
        decimal? crossBorderFlow)
    {
        switch (production)
        {
            case null:
                errors.Add(TimeSeriesDataValidationError.ProductionMissing);
                break;
            case < 0:
                errors.Add(TimeSeriesDataValidationError.ProductionNegative);
                break;
        }

        if (HasTooManyDecimalPlaces(production))
        {
            errors.Add(TimeSeriesDataValidationError.ProductionTooManyDecimalPlaces);
        }

        switch (consumption)
        {
            case null:
                errors.Add(TimeSeriesDataValidationError.ConsumptionMissing);
                break;
            case < 0:
                errors.Add(TimeSeriesDataValidationError.ConsumptionNegative);
                break;
        }

        if (HasTooManyDecimalPlaces(consumption))
        {
            errors.Add(TimeSeriesDataValidationError.ConsumptionTooManyDecimalPlaces);
        }

        if (crossBorderFlow is null)
        {
            errors.Add(TimeSeriesDataValidationError.CrossBorderFlowMissing);
        }
        else if (HasTooManyDecimalPlaces(crossBorderFlow))
        {
            errors.Add(TimeSeriesDataValidationError.CrossBorderFlowTooManyDecimalPlaces);
        }
    }

    // Trailing zeros don't count, so 3980.2000 has one decimal place.
    private static bool HasTooManyDecimalPlaces(decimal? value) =>
        value is { } number && number != Math.Round(number, MaxDecimalPlaces);
}
