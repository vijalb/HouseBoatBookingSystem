using HouseBoat.Application.Features.AI.Chat;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIController : ControllerBase
{
    private readonly ChatWithAIHandler _handler;

    public AIController(ChatWithAIHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatWithAIQuery query,
        CancellationToken cancellationToken)
    {
        var response = await _handler.Handle(
            query,
            cancellationToken);

        return Ok(new
        {
            response
        });
    }
}