using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class AdvertisedPackageConfiguration : IEntityTypeConfiguration<AdvertisedPackage>
{
    public void Configure(EntityTypeBuilder<AdvertisedPackage> entity)
    {
        entity.HasKey(e => e.PackageId).HasName("PK__Advertis__322035CCD24D6FCA");

        entity.ToTable("AdvertisedPackage");

        entity.Property(e => e.AdvertisedPrice).HasColumnType("decimal(10, 2)");
        entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Exclusions).HasMaxLength(500);
        entity.Property(e => e.GracePeriodDays).HasDefaultValue(1);
        entity.Property(e => e.Inclusions).HasMaxLength(500);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.Name).HasMaxLength(100);

        entity.HasOne(d => d.AdvertisedCurrency).WithMany(p => p.AdvertisedPackages)
            .HasForeignKey(d => d.AdvertisedCurrencyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_AdvertisedPackage_Currency");

        entity.HasOne(d => d.AuthorizedByEmployee).WithMany(p => p.AdvertisedPackages)
            .HasForeignKey(d => d.AuthorizedByEmployeeId)
            .HasConstraintName("FK_AdvertisedPackage_Employee");

        entity.HasOne(d => d.Status).WithMany(p => p.AdvertisedPackages)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_AdvertisedPackage_Status");
    }
}