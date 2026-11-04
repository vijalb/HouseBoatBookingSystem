using HouseBoat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Persistence.Configurations
{
    public class HouseboatConfiguration : IEntityTypeConfiguration<Houseboat>
    {
        public void Configure(EntityTypeBuilder<Houseboat> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Location)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.PricePerNight)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Capacity)
                .IsRequired();

            builder.Property(x => x.Bedrooms)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.HasAc).IsRequired();
        }
    }
}
