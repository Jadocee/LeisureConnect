using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BillPaymentConfiguration : IEntityTypeConfiguration<BillPayment>
{
    public void Configure(EntityTypeBuilder<BillPayment> entity)
    {
        entity.HasKey(e => e.BillPaymentId).HasName("PK__BillPaym__9BD090C7C98CABCD");

        entity.ToTable("BillPayment");

        entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
        entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");

        entity.HasOne(d => d.Bill).WithMany(p => p.BillPayments)
            .HasForeignKey(d => d.BillId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BillPayment_Bill");

        entity.HasOne(d => d.PaymentInformation).WithMany(p => p.BillPayments)
            .HasForeignKey(d => d.PaymentInformationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BillPayment_PaymentInformation");
    }
}