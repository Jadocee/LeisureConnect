using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BillChargeConfiguration : IEntityTypeConfiguration<BillCharge>
{
    public void Configure(EntityTypeBuilder<BillCharge> entity)
    {
        entity.HasKey(e => e.BillChargeId).HasName("PK__BillChar__EACED983A82C550F");

        entity.ToTable("BillCharge");

        entity.HasIndex(e => new { e.BillId, e.ChargeId }, "UQ_BillCharge").IsUnique();

        entity.HasOne(d => d.Bill).WithMany(p => p.BillCharges)
            .HasForeignKey(d => d.BillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BillCharge_Bill");

        entity.HasOne(d => d.Charge).WithMany(p => p.BillCharges)
            .HasForeignKey(d => d.ChargeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BillCharge_Charge");
    }
}