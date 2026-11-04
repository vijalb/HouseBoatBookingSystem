using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<Houseboat> Houseboats { get; }

    DbSet<Booking> Bookings { get; }

    DbSet<Review> Reviews { get; }

    DbSet<Notification> Notifications { get; }

    DbSet <BookingConversation> BookingConversations { get; }
    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}