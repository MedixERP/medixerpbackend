using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class DrugOrderItemConfiguration
    : IEntityTypeConfiguration<DrugOrderItem>
{
    public void Configure(EntityTypeBuilder<DrugOrderItem> builder)
    {
        builder.ToTable("DrugOrderItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Total)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.DrugOrder)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.DrugOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}