using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Notifications.MarkAllAsRead;

public class MarkAllNotificationsAsReadHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public MarkAllNotificationsAsReadHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task Handle(
        MarkAllNotificationsAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var notifications = await _context.Notifications
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            return;
        }

        var readAt = DateTime.UtcNow;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = readAt;
        }

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}