using HouseBoat.Application.Common.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;

namespace HouseBoat.Infrastructure.AI.Ollama;

public class OllamaChatService : IAIChatService
{
    private readonly IChatClient _chatClient;
    private static readonly TimeSpan AiTimeout =
    TimeSpan.FromSeconds(120);
    public OllamaChatService(IOptions<OllamaOptions> options)
    {
        var settings = options.Value;

        _chatClient = new OllamaApiClient(
            new Uri(settings.Endpoint),
            settings.Model);
    }

    public async Task<string> GetResponseAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = CreateTimeoutToken(cancellationToken);

        var response = await _chatClient.GetResponseAsync(
            prompt,
            cancellationToken: timeoutCts.Token);

        return response.Text ?? string.Empty;
    }

    public async Task<T> GetStructuredResponseAsync<T>(
        string prompt,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("STRUCTURED AI -> START");

        using var timeoutCts = CreateTimeoutToken(cancellationToken);

        var response = await _chatClient.GetResponseAsync<T>(
            prompt,
            cancellationToken: timeoutCts.Token);
        Console.WriteLine("STRUCTURED AI -> END");
        return response.Result;

        
    }

    public async Task<string> GetResponseWithToolsAsync(
    string prompt,
    IEnumerable<AIFunction> tools,
    CancellationToken cancellationToken)
    {
        var client = new ChatClientBuilder(_chatClient)
            .UseFunctionInvocation()
            .Build();

        var options = new ChatOptions
        {
            Tools = tools
        .Cast<AITool>()
        .ToList()
        };

        var response = await client.GetResponseAsync(
            prompt,
            options,
            cancellationToken);

        return response.Text ?? string.Empty;
    }

    private static CancellationTokenSource CreateTimeoutToken(
    CancellationToken cancellationToken)
    {
        var cts = CancellationTokenSource
            .CreateLinkedTokenSource(cancellationToken);

        cts.CancelAfter(AiTimeout);

        return cts;
    }
}