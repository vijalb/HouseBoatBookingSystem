using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Notifications.MarkAsRead;

public class MarkNotificationAsReadHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public MarkNotificationAsReadHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                x =>
                    x.Id == command.NotificationId &&
                    x.UserId == userId,
                cancellationToken);

        if (notification is null)
        {
            throw new KeyNotFoundException(
                "Notification not found.");
        }

        if (notification.IsRead)
        {
            throw new InvalidOperationException(
                "Notification is already marked as read.");
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}