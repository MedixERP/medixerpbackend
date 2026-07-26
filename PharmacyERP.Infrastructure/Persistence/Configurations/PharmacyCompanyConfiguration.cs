using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

namespace PharmacyERP.Infrastructure.Persistence.Configurations;

public class PharmacyCompanyConfiguration
    : IEntityTypeConfiguration<PharmacyCompany>
{
    public void Configure(EntityTypeBuilder<PharmacyCompany> builder)
    {
        builder.ToTable("PharmacyCompanies");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Email)
            .HasMaxLength(150);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Address)
            .HasMaxLength(300);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<PharmacyCompany>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}