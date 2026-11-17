using System.Globalization;
using NordicRCC.AcceptanceTests.Drivers;
using Reqnroll;

namespace NordicRCC.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class TimeSeriesDataStepDefinitions(ITimeSeriesDataDriver timeSeriesData)
{
    private IReadOnlyList<TimeSeriesData> _fetched = [];

    [Given("the following time series data has been recorded")]
    public Task GivenTheFollowingTimeSeriesDataHasBeenRecorded(DataTable table) =>
        timeSeriesData.RecordAsync(Parse(table));

    [Given("no time series data has been recorded")]
    public void GivenNoTimeSeriesDataHasBeenRecorded()
    {
        // A newly started system holds no time series data.
    }

    [When("all time series data is fetched")]
    public async Task WhenAllTimeSeriesDataIsFetched() => _fetched = await timeSeriesData.FetchAllAsync();

    [Then("the following time series data is returned")]
    public async Task ThenTheFollowingTimeSeriesDataIsReturned(DataTable table)
    {
        // The feature doesn't say in which order the data is returned.
        await Assert.That(InOrder(_fetched)).IsEquivalentTo(InOrder(Parse(table)));
    }

    [Then("no time series data is returned")]
    public async Task ThenNoTimeSeriesDataIsReturned() => await Assert.That(_fetched).IsEmpty();

    private static List<TimeSeriesData> Parse(DataTable table) =>
    [
        .. table.Rows.Select(row => new TimeSeriesData(
            DateTimeOffset.Parse(row["Timestamp"], CultureInfo.InvariantCulture),
            row["Bidding zone"],
            decimal.Parse(row["Production"], CultureInfo.InvariantCulture),
            decimal.Parse(row["Consumption"], CultureInfo.InvariantCulture),
            decimal.Parse(row["Cross-border flow"], CultureInfo.InvariantCulture))),
    ];

    private static List<TimeSeriesData> InOrder(IEnumerable<TimeSeriesData> data) =>
        [.. data.OrderBy(item => item.Timestamp).ThenBy(item => item.BiddingZone, StringComparer.Ordinal)];
}
