using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Domain.Entities.AI
{
    public class BookingConversation
    {
        public Guid ConversationId { get; set; }

        public string StateJson { get; set; } = "{}";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
