using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BookingGuestConfiguration : IEntityTypeConfiguration<BookingGuest>
{
    public void Configure(EntityTypeBuilder<BookingGuest> entity)
    {
        entity.HasKey(e => e.BookingGuestId).HasName("PK__BookingG__1101F9D8D8BD9FE6");

        entity.ToTable("BookingGuest");

        entity.HasIndex(e => new { e.BookingId, e.GuestId }, "UQ_BookingGuest").IsUnique();

        entity.HasOne(d => d.Booking).WithMany(p => p.BookingGuests)
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BookingGuest_Booking");

        entity.HasOne(d => d.Guest).WithMany(p => p.BookingGuests)
            .HasForeignKey(d => d.GuestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BookingGuest_Guest");
    }
}