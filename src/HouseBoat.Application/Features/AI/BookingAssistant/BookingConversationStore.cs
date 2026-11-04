using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace HouseBoat.Application.Features.AI.BookingAssistant
{
    public class BookingConversationStore : IBookingConversationStore
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<BookingConversationStore> _logger;
        public BookingConversationStore(IApplicationDbContext dbContext, ILogger<BookingConversationStore> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<BookingConversationState> GetOrCreateAsync(
            Guid conversationId,
            CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.BookingConversations
                .FirstOrDefaultAsync(
                    x => x.ConversationId == conversationId,
                    cancellationToken);

            if (entity is not null)
            {
                try
                {
                    // Expire conversations after 24 hours of inactivity.
                    if (entity.UpdatedAt <= DateTime.UtcNow.AddHours(-24))
                    {
                        BookingConversationState? existingState = null;

                        try
                        {
                            existingState =
                                JsonSerializer.Deserialize<BookingConversationState>(
                                    entity.StateJson);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning(
                                ex,
                                "Invalid conversation state JSON for {ConversationId}.",
                                conversationId);
                        }

                        // Preserve completed bookings to prevent duplicate creation.
                        if (existingState?.IsBookingCompleted == true &&
                            existingState.CreatedBookingId.HasValue)
                        {
                            return existingState;
                        }

                        // Reset expired or corrupted incomplete conversations.
                        var expiredState = new BookingConversationState
                        {
                            ConversationId = conversationId
                        };

                        entity.StateJson = JsonSerializer.Serialize(expiredState);
                        entity.UpdatedAt = DateTime.UtcNow;

                        await _dbContext.SaveChangesAsync(cancellationToken);

                        _logger.LogInformation(
                            "Conversation {ConversationId} expired due to inactivity.",
                            conversationId);

                        return expiredState;
                    }


                    var deserializedState =
                    JsonSerializer.Deserialize<BookingConversationState>(
                        entity.StateJson);                                      

                    if (deserializedState is not null)
                    {
                        return deserializedState;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Invalid conversation state JSON for {ConversationId}. Resetting state.",
                        conversationId);
                }

                // Recover by creating a clean conversation state.
                var resetState = new BookingConversationState
                {
                    ConversationId = conversationId
                };

                entity.StateJson = JsonSerializer.Serialize(resetState);
                entity.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync(cancellationToken);

                return resetState;
            }

            var state = new BookingConversationState
            {
                ConversationId = conversationId
            };

            entity = new BookingConversation
            {
                ConversationId = conversationId,
                StateJson = JsonSerializer.Serialize(state),
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.BookingConversations.Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return state;
        }

        public async Task SaveAsync(
            BookingConversationState state,
            CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.BookingConversations
                .FirstOrDefaultAsync(
                    x => x.ConversationId == state.ConversationId,
                    cancellationToken);

            if (entity is null)
            {
                entity = new BookingConversation
                {
                    ConversationId = state.ConversationId
                };

                _dbContext.BookingConversations.Add(entity);
            }

            entity.StateJson = JsonSerializer.Serialize(state);
            entity.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
