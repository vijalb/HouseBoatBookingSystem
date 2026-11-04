using Microsoft.Extensions.AI;

namespace HouseBoat.Application.Common.Interfaces;

public interface IAIChatService
{
    Task<string> GetResponseAsync(
        string prompt,
        CancellationToken cancellationToken);

    Task<T> GetStructuredResponseAsync<T>(
        string prompt,
        CancellationToken cancellationToken);

    Task<string> GetResponseWithToolsAsync(
        string prompt,
        IEnumerable<AIFunction> tools,
        CancellationToken cancellationToken);
}