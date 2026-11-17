using Microsoft.EntityFrameworkCore;

namespace NordicRCC.Domain.TimeSeries;

public sealed class TimeSeriesDataService(INordicRccDbContext database) : ITimeSeriesDataService
{
    public async Task<IReadOnlyList<TimeSeriesData>> GetAsync(
        TimeSeriesDataFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = database.TimeSeriesData.AsNoTracking();

        if (filter.BiddingZone is not null)
        {
            query = query.Where(data => data.BiddingZone == filter.BiddingZone);
        }

        return await query
            .OrderBy(data => data.Timestamp)
            .ThenBy(data => data.BiddingZone)
            .ToListAsync(cancellationToken);
    }
}
