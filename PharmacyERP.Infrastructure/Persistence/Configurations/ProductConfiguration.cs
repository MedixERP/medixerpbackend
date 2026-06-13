using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ScientificName)
            .HasMaxLength(200);

        
        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.Barcode)
            .IsUnique();

       
        builder.Property(x => x.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);

        
        builder.Property(x => x.BarcodeImage)
            .HasColumnType("varbinary(max)");

        builder.Property(x => x.QrCodeImage)
            .HasColumnType("varbinary(max)");

       
        builder.Property(x => x.RowVersion)
            .IsRowVersion();

       
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}