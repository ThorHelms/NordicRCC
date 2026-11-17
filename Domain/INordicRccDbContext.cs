using Microsoft.EntityFrameworkCore;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Domain;

/// <summary>
/// The application's database. The infrastructure decides where and how the data is stored.
/// </summary>
public interface INordicRccDbContext
{
    DbSet<TimeSeriesData> TimeSeriesData { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
