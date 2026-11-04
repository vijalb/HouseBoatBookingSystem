using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Domain.Entities
{
    public class Houseboat : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public decimal PricePerNight { get; set; }

        public bool HasAc { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Bedrooms { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Review> Reviews { get; set; }
    = new List<Review>();

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
