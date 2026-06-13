

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

     
     builder.Property(x => x.Phone)
     .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}