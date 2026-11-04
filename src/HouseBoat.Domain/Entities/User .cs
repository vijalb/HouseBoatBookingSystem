using HouseBoat.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public ICollection<Booking> Bookings { get; set; }= new List<Booking>();

        public ICollection<Review> Reviews { get; set; }= new List<Review>();

        public UserRole Role { get; set; } = UserRole.Customer;
    }
}
