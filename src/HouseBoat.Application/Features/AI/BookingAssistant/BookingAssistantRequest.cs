using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public record BookingAssistantRequest(
    Guid ConversationId,
    string Message);
}
