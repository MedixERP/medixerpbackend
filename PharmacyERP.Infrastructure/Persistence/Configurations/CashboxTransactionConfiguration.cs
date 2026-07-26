using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class CashboxTransactionConfiguration
    : IEntityTypeConfiguration<CashboxTransaction>
{
    public void Configure(EntityTypeBuilder<CashboxTransaction> builder)
    {
        builder.ToTable("CashboxTransactions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount)
            .HasPrecision(18, 2);
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        builder.Property(x => x.ReferenceType)
            .HasMaxLength(100);
        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}