using HouseBoat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid HouseBoatId { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int GuestCount { get; set; }

        public decimal TotalAmount { get; set; }

        public BookingStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;

        public Houseboat HouseBoat { get; set; } = null!;

    }
}
