using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> entity)
    {
        entity.HasKey(e => e.ChargeId).HasName("PK__Charge__17FC361B7293F0D5");

        entity.ToTable("Charge");

        entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
        entity.Property(e => e.ChargeDate).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Description).HasMaxLength(255);

        entity.HasOne(d => d.Booking).WithMany(p => p.Charges)
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Charge_Booking");

        entity.HasOne(d => d.ChargeType).WithMany(p => p.Charges)
            .HasForeignKey(d => d.ChargeTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Charge_ChargeType");

        entity.HasOne(d => d.Currency).WithMany(p => p.Charges)
            .HasForeignKey(d => d.CurrencyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Charge_Currency");

        entity.HasOne(d => d.Employee).WithMany(p => p.Charges)
            .HasForeignKey(d => d.EmployeeId)
            .HasConstraintName("FK_Charge_Employee");

        entity.HasOne(d => d.ServiceItem).WithMany(p => p.Charges)
            .HasForeignKey(d => d.ServiceItemId)
            .HasConstraintName("FK_Charge_ServiceItem");
    }
}