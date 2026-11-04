using HouseBoat.Application.Common.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;

namespace HouseBoat.Infrastructure.AI.OpenAI;

public class OpenAIChatService : IAIChatService
{
    private readonly IChatClient _chatClient;
    private readonly OpenAIOptions _options;

    public OpenAIChatService(IOptions<OpenAIOptions> options)
    {
        _options = options.Value;

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(_options.ApiKey));

        _chatClient = openAIClient
            .GetChatClient(_options.Model)
            .AsIChatClient();
    }

    public async Task<string> GetResponseAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var response = await _chatClient.GetResponseAsync(
            prompt,
            cancellationToken: cancellationToken);

        return response.Text ?? string.Empty;
    }

    public async Task<T> GetStructuredResponseAsync<T>(
    string prompt,
    CancellationToken cancellationToken)
    {
        var response = await _chatClient.GetResponseAsync<T>(
            prompt,
            cancellationToken: cancellationToken);

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
}