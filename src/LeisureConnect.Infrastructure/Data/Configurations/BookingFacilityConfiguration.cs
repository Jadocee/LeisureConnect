using LeisureConnect.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeisureConnect.Infrastructure.Data.Configurations;

public class BookingFacilityConfiguration : IEntityTypeConfiguration<BookingFacility>
{
    public void Configure(EntityTypeBuilder<BookingFacility> entity)
    {
        entity.HasKey(e => e.BookingFacilityId).HasName("PK__BookingF__0DE4427C1874886B");

        entity.ToTable("BookingFacility");

        entity.HasOne(d => d.Booking).WithMany(p => p.BookingFacilities)
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BookingFacility_Booking");

        entity.HasOne(d => d.Facility).WithMany(p => p.BookingFacilities)
            .HasForeignKey(d => d.FacilityId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_BookingFacility_Facility");
    }
}