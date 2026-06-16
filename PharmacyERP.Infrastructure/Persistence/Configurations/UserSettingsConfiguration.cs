using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyERP.Domain.Entities;

public class UserSettingsConfiguration
    : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(
        EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Language)
            .HasMaxLength(10);

        builder.Property(x => x.Theme)
            .HasMaxLength(20);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Settings)
            .HasForeignKey<UserSettings>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}