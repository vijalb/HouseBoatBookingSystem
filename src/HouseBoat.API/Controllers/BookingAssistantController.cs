using Azure.Core;
using HouseBoat.Application.Features.AI.BookingAssistant;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingAssistantController : ControllerBase
{
    private readonly BookingAssistantHandler _handler;

    public BookingAssistantController(
        BookingAssistantHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(
       BookingAssistantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _handler.Handle(
    request.ConversationId,
    request.Message,
    cancellationToken);

        return Ok(result);
    }
}