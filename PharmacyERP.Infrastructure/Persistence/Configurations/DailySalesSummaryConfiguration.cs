using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class DailySalesSummaryConfiguration : IEntityTypeConfiguration<DailySalesSummary>
{
    public void Configure(EntityTypeBuilder<DailySalesSummary> builder)
    {
        builder.ToTable("DailySalesSummaries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalSales)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.Date)
            .IsUnique();
    }
}