using Microsoft.EntityFrameworkCore;
using NordicRCC.Domain;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Infrastructure.Database;

public sealed class NordicRccDbContext(DbContextOptions<NordicRccDbContext> options)
    : DbContext(options), INordicRccDbContext
{
    public DbSet<TimeSeriesData> TimeSeriesData => Set<TimeSeriesData>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NordicRccDbContext).Assembly);
}
