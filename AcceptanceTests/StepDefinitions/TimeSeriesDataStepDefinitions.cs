using System.Globalization;
using NordicRCC.AcceptanceTests.Drivers;
using Reqnroll;

namespace NordicRCC.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class TimeSeriesDataStepDefinitions(ITimeSeriesDataDriver timeSeriesData)
{
    private static readonly Dictionary<string, TimeSeriesDataRejectionReason> RejectionReasons = new()
    {
        ["the timestamp is missing"] = TimeSeriesDataRejectionReason.TimestampMissing,
        ["the bidding zone is missing"] = TimeSeriesDataRejectionReason.BiddingZoneMissing,
        ["the production is missing"] = TimeSeriesDataRejectionReason.ProductionMissing,
        ["the consumption is missing"] = TimeSeriesDataRejectionReason.ConsumptionMissing,
        ["the cross-border flow is missing"] = TimeSeriesDataRejectionReason.CrossBorderFlowMissing,
        ["the bidding zone is unknown"] = TimeSeriesDataRejectionReason.BiddingZoneUnknown,
        ["the production is negative"] = TimeSeriesDataRejectionReason.ProductionNegative,
        ["the consumption is negative"] = TimeSeriesDataRejectionReason.ConsumptionNegative,
        ["the production has more than three decimal places"] =
            TimeSeriesDataRejectionReason.ProductionTooManyDecimalPlaces,
        ["the consumption has more than three decimal places"] =
            TimeSeriesDataRejectionReason.ConsumptionTooManyDecimalPlaces,
        ["the cross-border flow has more than three decimal places"] =
            TimeSeriesDataRejectionReason.CrossBorderFlowTooManyDecimalPlaces,
        ["the bidding zone already has time series data at the timestamp"] =
            TimeSeriesDataRejectionReason.AlreadyExists,
    };

    private IReadOnlyList<TimeSeriesData> _fetched = [];
    private TimeSeriesDataCreation? _creation;

    [Given("the following time series data has been recorded")]
    public Task GivenTheFollowingTimeSeriesDataHasBeenRecorded(DataTable table) =>
        timeSeriesData.RecordAsync(Parse(table));

    [Given("no time series data has been recorded")]
    public void GivenNoTimeSeriesDataHasBeenRecorded()
    {
        // A newly started system holds no time series data.
    }

    [When("the following time series data entry is created")]
    public async Task WhenTheFollowingTimeSeriesDataEntryIsCreated(DataTable table)
    {
        if (table.Rows.Count != 1)
        {
            throw new ArgumentException(
                $"The step creates one time series data entry, but its table has {table.Rows.Count} rows.");
        }

        _creation = await timeSeriesData.CreateAsync(ParseNew(table.Rows[0]));
    }

    [When("all time series data is fetched")]
    public async Task WhenAllTimeSeriesDataIsFetched() => _fetched = await timeSeriesData.FetchAllAsync();

    [When("the time series data for bidding zone {string} is fetched")]
    public async Task WhenTheTimeSeriesDataForBiddingZoneIsFetched(string biddingZone) =>
        _fetched = await timeSeriesData.FetchForBiddingZoneAsync(biddingZone);

    [When("the time series data from {string} to {string} is fetched")]
    public async Task WhenTheTimeSeriesDataFromToIsFetched(string from, string to) =>
        _fetched = await timeSeriesData.FetchForPeriodAsync(ParseTimestamp(from), ParseTimestamp(to));

    [When("the time series data from {string} is fetched")]
    public async Task WhenTheTimeSeriesDataFromIsFetched(string from) =>
        _fetched = await timeSeriesData.FetchForPeriodAsync(ParseTimestamp(from), to: null);

    [When("the time series data up to {string} is fetched")]
    public async Task WhenTheTimeSeriesDataUpToIsFetched(string to) =>
        _fetched = await timeSeriesData.FetchForPeriodAsync(from: null, ParseTimestamp(to));

    [Then("the following time series data is returned")]
    public async Task ThenTheFollowingTimeSeriesDataIsReturned(DataTable table)
    {
        // The feature doesn't say in which order the data is returned.
        await Assert.That(InOrder(_fetched)).IsEquivalentTo(InOrder(Parse(table)));
    }

    [Then("no time series data is returned")]
    public async Task ThenNoTimeSeriesDataIsReturned() => await Assert.That(_fetched).IsEmpty();

    [Then("the time series data entry is accepted")]
    public async Task ThenTheTimeSeriesDataEntryIsAccepted() =>
        await Assert.That(Creation.RejectionReasons).IsEmpty();

    [Then("the time series data entry is rejected because {string}")]
    public async Task ThenTheTimeSeriesDataEntryIsRejectedBecause(string reason)
    {
        if (!RejectionReasons.TryGetValue(reason, out var expected))
        {
            throw new ArgumentException(
                $"'{reason}' isn't a known rejection reason. Known reasons: {string.Join("; ", RejectionReasons.Keys)}.");
        }

        await Assert.That(Creation.RejectionReasons).IsEquivalentTo([expected]);
    }

    [Then("the system holds the following time series data")]
    public async Task ThenTheSystemHoldsTheFollowingTimeSeriesData(DataTable table)
    {
        var held = await timeSeriesData.FetchAllAsync();
        await Assert.That(InOrder(held)).IsEquivalentTo(InOrder(Parse(table)));
    }

    [Then("the system holds no time series data")]
    public async Task ThenTheSystemHoldsNoTimeSeriesData() =>
        await Assert.That(await timeSeriesData.FetchAllAsync()).IsEmpty();

    private TimeSeriesDataCreation Creation =>
        _creation ?? throw new InvalidOperationException("No time series data entry has been created in the scenario.");

    // An empty cell leaves the value out of the entry.
    private static NewTimeSeriesData ParseNew(DataTableRow row) =>
        new(
            OrNull(row["Timestamp"], ParseTimestamp),
            string.IsNullOrWhiteSpace(row["Bidding zone"]) ? null : row["Bidding zone"],
            OrNull(row["Production"], ParseDecimal),
            OrNull(row["Consumption"], ParseDecimal),
            OrNull(row["Cross-border flow"], ParseDecimal));

    private static T? OrNull<T>(string value, Func<string, T> parse)
        where T : struct =>
        string.IsNullOrWhiteSpace(value) ? null : parse(value);

    private static decimal ParseDecimal(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);

    private static List<TimeSeriesData> Parse(DataTable table) =>
    [
        .. table.Rows.Select(row => new TimeSeriesData(
            ParseTimestamp(row["Timestamp"]),
            row["Bidding zone"],
            decimal.Parse(row["Production"], CultureInfo.InvariantCulture),
            decimal.Parse(row["Consumption"], CultureInfo.InvariantCulture),
            decimal.Parse(row["Cross-border flow"], CultureInfo.InvariantCulture))),
    ];

    private static DateTimeOffset ParseTimestamp(string value) =>
        DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);

    private static List<TimeSeriesData> InOrder(IEnumerable<TimeSeriesData> data) =>
        [.. data.OrderBy(item => item.Timestamp).ThenBy(item => item.BiddingZone, StringComparer.Ordinal)];
}
