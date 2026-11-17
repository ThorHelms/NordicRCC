using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Application.Api.TimeSeries;

public static class TimeSeriesDataEndpoints
{
    /// <summary>
    /// The problem details extension that lists the machine-readable codes of the validation errors.
    /// </summary>
    public const string ErrorCodesExtension = "errorCodes";

    private const string GetTimeSeriesDataByIdRoute = "GetTimeSeriesDataById";

    public static IEndpointRouteBuilder MapTimeSeriesDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/time-series-data", GetTimeSeriesDataAsync)
            .WithName("GetTimeSeriesData");

        endpoints.MapGet("/time-series-data/{id:long}", GetTimeSeriesDataByIdAsync)
            .WithName(GetTimeSeriesDataByIdRoute);

        // ProblemHttpResult can carry any status code, so it doesn't document one by itself.
        endpoints.MapPost("/time-series-data", CreateTimeSeriesDataAsync)
            .WithName("CreateTimeSeriesData")
            .ProducesProblem(StatusCodes.Status409Conflict);

        endpoints.MapPut("/time-series-data/{id:long}", UpdateTimeSeriesDataAsync)
            .WithName("UpdateTimeSeriesData");

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

    private static async Task<Results<Ok<TimeSeriesDataDto>, NotFound<ProblemDetails>>> GetTimeSeriesDataByIdAsync(
        long id,
        ITimeSeriesDataService timeSeriesData,
        CancellationToken cancellationToken)
    {
        var result = await timeSeriesData.GetByIdAsync(id, cancellationToken);

        return result switch
        {
            TimeSeriesData entry => TypedResults.Ok(TimeSeriesDataDto.From(entry)),
            TimeSeriesDataNotFound notFound => NotFound(notFound),
        };
    }

    private static async Task<Results<CreatedAtRoute<TimeSeriesDataDto>, ValidationProblem, ProblemHttpResult>>
        CreateTimeSeriesDataAsync(
            CreateTimeSeriesDataRequest request,
            ITimeSeriesDataService timeSeriesData,
            CancellationToken cancellationToken)
    {
        var result = await timeSeriesData.CreateAsync(request.ToNewTimeSeriesData(), cancellationToken);

        return result switch
        {
            TimeSeriesData created => TypedResults.CreatedAtRoute(
                TimeSeriesDataDto.From(created),
                GetTimeSeriesDataByIdRoute,
                new { id = created.Id }),
            // A duplicate entry is valid on its own, but conflicts with the data that the system already holds.
            TimeSeriesDataValidationFailed { Errors: [TimeSeriesDataValidationError.AlreadyExists] } failed =>
                TypedResults.Problem(
                    title: "The time series data already exists.",
                    detail: Describe(TimeSeriesDataValidationError.AlreadyExists).Message,
                    statusCode: StatusCodes.Status409Conflict,
                    extensions: ErrorCodes(failed)),
            TimeSeriesDataValidationFailed failed => ToValidationProblem(failed),
        };
    }

    private static async Task<Results<Ok<TimeSeriesDataDto>, ValidationProblem, NotFound<ProblemDetails>>>
        UpdateTimeSeriesDataAsync(
            long id,
            UpdateTimeSeriesDataRequest request,
            ITimeSeriesDataService timeSeriesData,
            CancellationToken cancellationToken)
    {
        var result = await timeSeriesData.UpdateAsync(id, request.ToUpdatedTimeSeriesData(), cancellationToken);

        return result switch
        {
            TimeSeriesData updated => TypedResults.Ok(TimeSeriesDataDto.From(updated)),
            TimeSeriesDataValidationFailed failed => ToValidationProblem(failed),
            TimeSeriesDataNotFound notFound => NotFound(notFound),
        };
    }

    private static NotFound<ProblemDetails> NotFound(TimeSeriesDataNotFound notFound) =>
        TypedResults.NotFound(new ProblemDetails
        {
            Title = "The time series data was not found.",
            Detail = $"There is no time series data entry with id {notFound.Id}.",
            Status = StatusCodes.Status404NotFound,
        });

    private static ValidationProblem ToValidationProblem(TimeSeriesDataValidationFailed failed)
    {
        var errors = failed.Errors
            .Select(Describe)
            .GroupBy(error => error.Field, error => error.Message)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return TypedResults.ValidationProblem(
            errors,
            title: "The time series data is invalid.",
            extensions: ErrorCodes(failed));
    }

    private static Dictionary<string, object?> ErrorCodes(TimeSeriesDataValidationFailed failed) =>
        new()
        {
            [ErrorCodesExtension] = failed.Errors.Select(error => error.ToString()).ToArray(),
        };

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
