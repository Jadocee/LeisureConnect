using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> entity)
    {
        entity.HasKey(e => e.BillId).HasName("PK__Bill__11F2FC6A46FDB86B");

        entity.ToTable("Bill");

        entity.Property(e => e.DepositAmount)
            .HasDefaultValue(0m)
            .HasColumnType("decimal(10, 2)");
        entity.Property(e => e.DiscountAmount)
            .HasDefaultValue(0m)
            .HasColumnType("decimal(10, 2)");
        entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
        entity.Property(e => e.HeadOfficeAuthorizationStatus).HasMaxLength(50);
        entity.Property(e => e.IssuedDate).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Notes).HasMaxLength(500);
        entity.Property(e => e.PaidAmount).HasColumnType("decimal(10, 2)");
        entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

        entity.HasOne(d => d.Currency).WithMany(p => p.Bills)
            .HasForeignKey(d => d.CurrencyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Bill_Currency");

        entity.HasOne(d => d.DiscountAuthorizedByEmployee).WithMany(p => p.Bills)
            .HasForeignKey(d => d.DiscountAuthorizedByEmployeeId)
            .HasConstraintName("FK_Bill_Employee");

        entity.HasOne(d => d.Reservation).WithMany(p => p.Bills)
            .HasForeignKey(d => d.ReservationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Bill_Reservation");

        entity.HasOne(d => d.Status).WithMany(p => p.Bills)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Bill_Status");
    }
}