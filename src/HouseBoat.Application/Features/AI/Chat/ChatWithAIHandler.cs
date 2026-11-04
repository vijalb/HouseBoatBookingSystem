using HouseBoat.Application.Common.Interfaces;

namespace HouseBoat.Application.Features.AI.Chat;

public class ChatWithAIHandler
{
    private readonly IAIChatService _aiChatService;

    public ChatWithAIHandler(IAIChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    public async Task<string> Handle(
        ChatWithAIQuery query,
        CancellationToken cancellationToken)
    {
        return await _aiChatService.GetResponseAsync(
            query.Prompt,
            cancellationToken);
    }
}