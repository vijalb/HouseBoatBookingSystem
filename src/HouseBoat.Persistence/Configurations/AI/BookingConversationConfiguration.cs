using HouseBoat.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Persistence.Configurations.AI
{
    public class BookingConversationConfiguration
    : IEntityTypeConfiguration<BookingConversation>
    {
        public void Configure(
            EntityTypeBuilder<BookingConversation> builder)
        {
            builder.ToTable("BookingConversations");

            builder.HasKey(x => x.ConversationId);

            builder.Property(x => x.StateJson)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();
        }
    }
}
