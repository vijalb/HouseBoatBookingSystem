using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;

namespace HouseBoat.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;

    public NotificationService(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}