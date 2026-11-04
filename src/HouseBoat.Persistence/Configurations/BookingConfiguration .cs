using HouseBoat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HouseBoat.Persistence.Configurations;

public class BookingConfiguration
    : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CheckInDate)
            .IsRequired();

        builder.Property(x => x.CheckOutDate)
            .IsRequired();

        builder.Property(x => x.GuestCount)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
    .WithMany(x => x.Bookings)
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HouseBoat)
            .WithMany(x => x.Bookings)
            .HasForeignKey(x => x.HouseBoatId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}