using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Persistence.Context;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Houseboat> Houseboats => Set<Houseboat>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications { get; set; }

    public DbSet<BookingConversation> BookingConversations =>
    Set<BookingConversation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}