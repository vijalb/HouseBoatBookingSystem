using HouseBoat.Application.Features.Notifications.MarkAllAsRead;
using HouseBoat.Application.Features.Notifications.MarkAsRead;
using HouseBoat.Application.Features.Notifications.MyNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly GetMyNotificationsHandler _getMyNotificationsHandler;
    private readonly MarkNotificationAsReadHandler _markNotificationAsReadHandler;
    private readonly MarkAllNotificationsAsReadHandler
        _markAllNotificationsAsReadHandler;

    public NotificationsController(
        GetMyNotificationsHandler getMyNotificationsHandler,MarkNotificationAsReadHandler markNotificationAsReadHandler,
        MarkAllNotificationsAsReadHandler markAllNotificationsAsReadHandler)
    {
        _getMyNotificationsHandler = getMyNotificationsHandler;
        _markNotificationAsReadHandler = markNotificationAsReadHandler;
        _markAllNotificationsAsReadHandler = markAllNotificationsAsReadHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        CancellationToken cancellationToken)
    {
        var query = new GetMyNotificationsQuery();

        var notifications =
            await _getMyNotificationsHandler.Handle(
                query,
                cancellationToken);

        return Ok(notifications);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
    Guid id,
    CancellationToken cancellationToken)
    {
        var command =
            new MarkNotificationAsReadCommand(id);

        await _markNotificationAsReadHandler.Handle(
            command,
            cancellationToken);

        return Ok(new
        {
            message = "Notification marked as read."
        });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
    CancellationToken cancellationToken)
    {
        var command =
            new MarkAllNotificationsAsReadCommand();

        await _markAllNotificationsAsReadHandler.Handle(
            command,
            cancellationToken);

        return Ok(new
        {
            message = "All notifications marked as read."
        });
    }
}