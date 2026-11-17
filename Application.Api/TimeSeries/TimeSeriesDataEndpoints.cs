using Microsoft.AspNetCore.Http.HttpResults;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

public static class TimeSeriesDataEndpoints
{
    /// <summary>
    /// The problem details extension that lists the machine-readable codes of the validation errors.
    /// </summary>
    public const string ErrorCodesExtension = "errorCodes";

    public static IEndpointRouteBuilder MapTimeSeriesDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/time-series-data", GetTimeSeriesDataAsync)
            .WithName("GetTimeSeriesData");

        // ProblemHttpResult can carry any status code, so it doesn't document one by itself.
        endpoints.MapPost("/time-series-data", CreateTimeSeriesDataAsync)
            .WithName("CreateTimeSeriesData")
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static async Task<IReadOnlyList<TimeSeriesDataDto>> GetTimeSeriesDataAsync(
        [AsParameters] TimeSeriesDataRequest request,
        ITimeSeriesDataService timeSeriesData,
        CancellationToken cancellationToken)
    {
        var data = await timeSeriesData.GetAsync(request.ToFilter(), cancellationToken);

        return [.. data.Select(TimeSeriesDataDto.From)];
    }

    private static async Task<Results<Created<TimeSeriesDataDto>, ValidationProblem, ProblemHttpResult>>
        CreateTimeSeriesDataAsync(
            CreateTimeSeriesDataRequest request,
            ITimeSeriesDataService timeSeriesData,
            CancellationToken cancellationToken)
    {
        var result = await timeSeriesData.CreateAsync(request.ToNewTimeSeriesData(), cancellationToken);

        return result switch
        {
            TimeSeriesData created => TypedResults.Created((string?)null, TimeSeriesDataDto.From(created)),
            TimeSeriesDataValidationFailed failed => ToProblem(failed),
        };
    }

    private static Results<Created<TimeSeriesDataDto>, ValidationProblem, ProblemHttpResult> ToProblem(
        TimeSeriesDataValidationFailed failed)
    {
        var extensions = new Dictionary<string, object?>
        {
            [ErrorCodesExtension] = failed.Errors.Select(error => error.ToString()).ToArray(),
        };

        // A duplicate entry is valid on its own, but conflicts with the data that the system already holds.
        if (failed.Errors is [TimeSeriesDataValidationError.AlreadyExists])
        {
            return TypedResults.Problem(
                title: "The time series data already exists.",
                detail: Describe(TimeSeriesDataValidationError.AlreadyExists).Message,
                statusCode: StatusCodes.Status409Conflict,
                extensions: extensions);
        }

        var errors = failed.Errors
            .Select(Describe)
            .GroupBy(error => error.Field, error => error.Message)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return TypedResults.ValidationProblem(
            errors,
            title: "The time series data is invalid.",
            extensions: extensions);
    }

    private static (string Field, string Message) Describe(TimeSeriesDataValidationError error) => error switch
    {
        TimeSeriesDataValidationError.TimestampMissing => ("timestamp", "The timestamp is required."),
        TimeSeriesDataValidationError.BiddingZoneMissing => ("biddingZone", "The bidding zone is required."),
        TimeSeriesDataValidationError.ProductionMissing => ("production", "The production is required."),
        TimeSeriesDataValidationError.ConsumptionMissing => ("consumption", "The consumption is required."),
        TimeSeriesDataValidationError.CrossBorderFlowMissing =>
            ("crossBorderFlow", "The cross-border flow is required."),
        TimeSeriesDataValidationError.BiddingZoneUnknown =>
            ("biddingZone", $"The bidding zone must be one of {string.Join(", ", BiddingZones.Nordic)}."),
        TimeSeriesDataValidationError.ProductionNegative => ("production", "The production must not be negative."),
        TimeSeriesDataValidationError.ConsumptionNegative =>
            ("consumption", "The consumption must not be negative."),
        TimeSeriesDataValidationError.ProductionTooManyDecimalPlaces =>
            ("production", TooManyDecimalPlaces("production")),
        TimeSeriesDataValidationError.ConsumptionTooManyDecimalPlaces =>
            ("consumption", TooManyDecimalPlaces("consumption")),
        TimeSeriesDataValidationError.CrossBorderFlowTooManyDecimalPlaces =>
            ("crossBorderFlow", TooManyDecimalPlaces("cross-border flow")),
        TimeSeriesDataValidationError.AlreadyExists =>
            ("", "The bidding zone already has time series data at the timestamp."),
        _ => throw new ArgumentOutOfRangeException(nameof(error), error, "Unknown validation error."),
    };

    private static string TooManyDecimalPlaces(string value) =>
        $"The {value} must not have more than {TimeSeriesDataValidator.MaxDecimalPlaces} decimal places.";
}
