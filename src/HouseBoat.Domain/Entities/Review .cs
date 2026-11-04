using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid HouseBoatId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public User User { get; set; } = null!;

        public Houseboat HouseBoat { get; set; } = null!;
    }
}
