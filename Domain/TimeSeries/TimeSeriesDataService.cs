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

        if (filter.From is { } from)
        {
            query = query.Where(data => data.Timestamp >= from);
        }

        if (filter.To is { } to)
        {
            query = query.Where(data => data.Timestamp < to);
        }

        return await query
            .OrderBy(data => data.Timestamp)
            .ThenBy(data => data.BiddingZone)
            .ToListAsync(cancellationToken);
    }

    public async Task<GetTimeSeriesDataByIdResult> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entry = await database.TimeSeriesData
            .AsNoTracking()
            .SingleOrDefaultAsync(data => data.Id == id, cancellationToken);

        return entry is null ? new TimeSeriesDataNotFound(id) : entry;
    }

    public async Task<CreateTimeSeriesDataResult> CreateAsync(
        NewTimeSeriesData data,
        CancellationToken cancellationToken = default)
    {
        var errors = TimeSeriesDataValidator.Validate(data);

        if (errors.Count > 0)
        {
            return new TimeSeriesDataValidationFailed(errors);
        }

        var entry = new TimeSeriesData(
            data.Timestamp!.Value,
            data.BiddingZone!,
            data.Production!.Value,
            data.Consumption!.Value,
            data.CrossBorderFlow!.Value);

        var alreadyExists = await database.TimeSeriesData.AnyAsync(
            existing => existing.BiddingZone == entry.BiddingZone && existing.Timestamp == entry.Timestamp,
            cancellationToken);

        if (alreadyExists)
        {
            return new TimeSeriesDataValidationFailed([TimeSeriesDataValidationError.AlreadyExists]);
        }

        database.TimeSeriesData.Add(entry);
        await database.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<UpdateTimeSeriesDataResult> UpdateAsync(
        long id,
        UpdatedTimeSeriesData data,
        CancellationToken cancellationToken = default)
    {
        var existing = await database.TimeSeriesData
            .AsNoTracking()
            .SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken);

        if (existing is null)
        {
            return new TimeSeriesDataNotFound(id);
        }

        var errors = TimeSeriesDataValidator.Validate(data);

        if (errors.Count > 0)
        {
            return new TimeSeriesDataValidationFailed(errors);
        }

        // The entry is immutable, so a copy with the new values replaces it. The copy keeps the id, timestamp and
        // bidding zone of the existing entry.
        var updated = existing with
        {
            Production = data.Production!.Value,
            Consumption = data.Consumption!.Value,
            CrossBorderFlow = data.CrossBorderFlow!.Value,
        };

        database.TimeSeriesData.Update(updated);
        await database.SaveChangesAsync(cancellationToken);

        return updated;
    }

    public async Task<DeleteTimeSeriesDataResult> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var existing = await database.TimeSeriesData
            .SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken);

        if (existing is null)
        {
            return new TimeSeriesDataNotFound(id);
        }

        database.TimeSeriesData.Remove(existing);
        await database.SaveChangesAsync(cancellationToken);

        return new TimeSeriesDataDeleted(id);
    }
}
