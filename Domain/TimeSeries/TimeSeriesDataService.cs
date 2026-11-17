using Microsoft.EntityFrameworkCore;

namespace NordicRCC.Domain.TimeSeries;

public sealed class TimeSeriesDataService(INordicRccDbContext database) : ITimeSeriesDataService
{
    public async Task<IReadOnlyList<TimeSeriesData>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await database.TimeSeriesData
            .AsNoTracking()
            .OrderBy(data => data.Timestamp)
            .ThenBy(data => data.BiddingZone)
            .ToListAsync(cancellationToken);
}
