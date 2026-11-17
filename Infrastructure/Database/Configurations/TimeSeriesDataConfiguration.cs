using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NordicRCC.Domain.TimeSeries;

namespace NordicRCC.Infrastructure.Database.Configurations;

internal sealed class TimeSeriesDataConfiguration : IEntityTypeConfiguration<TimeSeriesData>
{
    public void Configure(EntityTypeBuilder<TimeSeriesData> builder)
    {
        builder.HasKey(data => data.Id);
        builder.Property(data => data.Id).ValueGeneratedOnAdd();

        builder.Property(data => data.Timestamp).IsRequired();
        builder.Property(data => data.BiddingZone).IsRequired().HasMaxLength(16);
        builder.Property(data => data.Production).HasPrecision(18, 3);
        builder.Property(data => data.Consumption).HasPrecision(18, 3);
        builder.Property(data => data.CrossBorderFlow).HasPrecision(18, 3);

        // A bidding zone has one set of data per point in time.
        builder.HasIndex(data => new { data.BiddingZone, data.Timestamp }).IsUnique();
        builder.HasIndex(data => data.Timestamp);
    }
}
