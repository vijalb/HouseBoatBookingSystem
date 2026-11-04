using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public class BookingConversationState
    {
        public Guid ConversationId { get; set; }

        public string? Location { get; set; }

        public string? HouseBoatName { get; set; }

        public DateTime? CheckInDate { get; set; }

        public DateTime? CheckOutDate { get; set; }

        public int? GuestCount { get; set; }

        public bool IsWaitingForBookingConfirmation { get; set; }

        public bool IsBookingCompleted { get; set; }
        public Guid? CreatedBookingId { get; set; }
    }
}
