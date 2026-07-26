using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class DrugOrderConfiguration
    : IEntityTypeConfiguration<DrugOrder>
{
    public void Configure(EntityTypeBuilder<DrugOrder> builder)
    {
        builder.ToTable("DrugOrders");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        builder.Property(x => x.SupplierName)
            .HasMaxLength(150);

        builder.Property(x => x.SupplierPhone)
            .HasMaxLength(20);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.PharmacyCompany)
            .WithMany(x => x.DrugOrders)
            .HasForeignKey(x => x.PharmacyCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}