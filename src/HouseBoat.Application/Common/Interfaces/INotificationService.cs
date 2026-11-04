namespace HouseBoat.Application.Common.Interfaces;

public interface INotificationService
{
    Task CreateAsync(
        Guid userId,
        string title,
        string message,
        CancellationToken cancellationToken);
}