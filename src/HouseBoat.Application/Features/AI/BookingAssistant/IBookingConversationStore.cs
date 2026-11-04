using System;
using System.Collections.Generic;
using System.Text;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public interface IBookingConversationStore
    {
        Task<BookingConversationState> GetOrCreateAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default);

        Task SaveAsync(
            BookingConversationState state,
            CancellationToken cancellationToken = default);
    }
}
