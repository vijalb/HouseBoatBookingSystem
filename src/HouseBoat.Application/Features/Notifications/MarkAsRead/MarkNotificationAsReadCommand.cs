namespace HouseBoat.Application.Features.Notifications.MarkAsRead;

public record MarkNotificationAsReadCommand(
    Guid NotificationId);