using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> entity)
    {
        entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED5ABABF90");

        entity.ToTable("Booking");

        entity.Property(e => e.CancellationFee).HasColumnType("decimal(10, 2)");
        entity.Property(e => e.Notes).HasMaxLength(500);
        entity.Property(e => e.Quantity).HasDefaultValue(1);

        entity.HasOne(d => d.Package).WithMany(p => p.Bookings)
            .HasForeignKey(d => d.PackageId)
            .HasConstraintName("FK_Booking_Package");

        entity.HasOne(d => d.Reservation).WithMany(p => p.Bookings)
            .HasForeignKey(d => d.ReservationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Booking_Reservation");

        entity.HasOne(d => d.ServiceItem).WithMany(p => p.Bookings)
            .HasForeignKey(d => d.ServiceItemId)
            .HasConstraintName("FK_Booking_ServiceItem");

        entity.HasOne(d => d.Status).WithMany(p => p.Bookings)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Booking_Status");
    }
}