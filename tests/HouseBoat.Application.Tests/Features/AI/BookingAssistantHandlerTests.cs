using FluentValidation;
using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Features.AI.BookingAssistant;
using HouseBoat.Application.Features.AI.BookingAssistant.Tools;
using HouseBoat.Application.Features.Bookings.Create;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Persistence;
using HouseBoat.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
namespace HouseBoat.Application.Tests.Features.AI;

public class BookingAssistantHandlerTests
{
    [Fact]
    public async Task Handle_WhenBookingAlreadyCompleted_ReturnsExistingBookingId()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var conversation = new BookingConversationState
        {
            ConversationId = conversationId,
            IsBookingCompleted = true,
            CreatedBookingId = bookingId
        };

        var conversationStore =
            new Mock<IBookingConversationStore>();

        conversationStore
            .Setup(x => x.GetOrCreateAsync(
                conversationId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var dbContext = new Mock<IApplicationDbContext>();
        var userContext = new Mock<IUserContext>();
        var notificationService = new Mock<INotificationService>();

        var availabilityTool =
            new SearchHouseBoatAvailabilityTool(dbContext.Object);

        var createBookingHandler = new CreateBookingHandler(
            dbContext.Object,
            userContext.Object,
            notificationService.Object);

        var aiChatService = new Mock<IAIChatService>();
        var intentValidator = new Mock<IValidator<BookingIntent>>();
        var logger =
            new Mock<ILogger<BookingAssistantHandler>>();

        var handler = new BookingAssistantHandler(
            aiChatService.Object,
            availabilityTool,
            new SearchAvailabilityRequestValidator(),
            conversationStore.Object,
            createBookingHandler,
            intentValidator.Object,
            logger.Object);

        // Act
        var result = await handler.Handle(
            conversationId,
            "Yes, please book it.",
            CancellationToken.None);

        // Assert
        Assert.Contains(
            bookingId.ToString(),
            result);

        aiChatService.Verify(
            x => x.GetStructuredResponseAsync<BookingIntent>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        conversationStore.Verify(
            x => x.SaveAsync(
                It.IsAny<BookingConversationState>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserConfirmsNaturally_ProcessesConfirmation()
    {

        // Arrange
        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var conversation = new BookingConversationState
        {
            ConversationId = conversationId,
            IsWaitingForBookingConfirmation = true,
            IsBookingCompleted = false,
            Location = "Alappuzha",
            HouseBoatName = "Kerala Queen Premium",
            CheckInDate = DateTime.UtcNow.Date.AddDays(10),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(12),
            GuestCount = 2
        };

        var conversationStore = new Mock<IBookingConversationStore>();

        conversationStore
            .Setup(x => x.GetOrCreateAsync(
                conversationId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        conversationStore
            .Setup(x => x.SaveAsync(
                It.IsAny<BookingConversationState>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        var houseBoat = new Houseboat
        {
            Id = Guid.NewGuid(),
            Name = "Kerala Queen Premium",
            Location = "Alappuzha",
            Capacity = 8,
            PricePerNight = 18000,
            Bedrooms = 4,
            HasAc = true,
            IsActive = true
        };

        dbContext.Houseboats.Add(houseBoat);
        await dbContext.SaveChangesAsync();

        var availabilityTool =
            new SearchHouseBoatAvailabilityTool(dbContext);

        var userContext = new Mock<IUserContext>();

        userContext
            .SetupGet(x => x.UserId)
            .Returns(userId);

        var notificationService = new Mock<INotificationService>();

        var createBookingHandler = new CreateBookingHandler(
            dbContext,
            userContext.Object,
            notificationService.Object);

        var aiChatService = new Mock<IAIChatService>();
        var intentValidator = new Mock<IValidator<BookingIntent>>();
        var logger = new Mock<ILogger<BookingAssistantHandler>>();

        var handler = new BookingAssistantHandler(
            aiChatService.Object,
            availabilityTool,
            new SearchAvailabilityRequestValidator(),
            conversationStore.Object,
            createBookingHandler,
            intentValidator.Object,
            logger.Object);

        // Act
        var result = await handler.Handle(
            conversationId,
            "Yes, please book it.",
            CancellationToken.None);

        Console.WriteLine($"HANDLER RESULT: {result}");
        Console.WriteLine($"IsBookingCompleted: {conversation.IsBookingCompleted}");
        Console.WriteLine($"CreatedBookingId: {conversation.CreatedBookingId}");
        // Assert
        Assert.Contains(
            "booking",
            result,
            StringComparison.OrdinalIgnoreCase);

        Assert.True(conversation.IsBookingCompleted);
        Assert.NotNull(conversation.CreatedBookingId);

        Assert.Equal(1, await dbContext.Bookings.CountAsync());

        conversationStore.Verify(
            x => x.SaveAsync(
                It.Is<BookingConversationState>(s =>
                    s.IsBookingCompleted &&
                    s.CreatedBookingId.HasValue),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserCancelsConfirmation_DoesNotCreateBooking()
    {
        // Arrange
        var conversationId = Guid.NewGuid();

        var conversation = new BookingConversationState
        {
            ConversationId = conversationId,
            IsWaitingForBookingConfirmation = true,
            IsBookingCompleted = false,
            Location = "Alappuzha",
            HouseBoatName = "Kerala Queen Premium",
            CheckInDate = DateTime.UtcNow.Date.AddDays(10),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(12),
            GuestCount = 2
        };

        var conversationStore = new Mock<IBookingConversationStore>();

        conversationStore
            .Setup(x => x.GetOrCreateAsync(
                conversationId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        conversationStore
            .Setup(x => x.SaveAsync(
                It.IsAny<BookingConversationState>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var aiChatService = new Mock<IAIChatService>();
        var intentValidator = new Mock<IValidator<BookingIntent>>();
        var logger = new Mock<ILogger<BookingAssistantHandler>>();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;

        using var dbContext = new ApplicationDbContext(options);

        var availabilityTool = new SearchHouseBoatAvailabilityTool(dbContext);

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var notificationService = new Mock<INotificationService>();

        var createBookingHandler = new CreateBookingHandler(
            dbContext,
            userContext.Object,
            notificationService.Object);
        var handler = new BookingAssistantHandler(
    aiChatService.Object,
    availabilityTool,
    new SearchAvailabilityRequestValidator(),
    conversationStore.Object,
    createBookingHandler,
    intentValidator.Object,
    logger.Object);

        // Act
        var result = await handler.Handle(
            conversationId,
            "No, cancel.",
            CancellationToken.None);

        // Assert
        Assert.Contains(
            "cancelled",
            result,
            StringComparison.OrdinalIgnoreCase);

        Assert.False(conversation.IsWaitingForBookingConfirmation);
        Assert.False(conversation.IsBookingCompleted);
        Assert.Null(conversation.CreatedBookingId);

        conversationStore.Verify(
            x => x.SaveAsync(
                It.IsAny<BookingConversationState>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        aiChatService.Verify(
            x => x.GetStructuredResponseAsync<BookingIntent>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserGivesInvalidConfirmation_AsksForValidResponse()
    {
        // Arrange
        var conversationId = Guid.NewGuid();

        var conversation = new BookingConversationState
        {
            ConversationId = conversationId,
            IsWaitingForBookingConfirmation = true,
            IsBookingCompleted = false,
            Location = "Alappuzha",
            HouseBoatName = "Kerala Queen Premium",
            CheckInDate = DateTime.UtcNow.Date.AddDays(10),
            CheckOutDate = DateTime.UtcNow.Date.AddDays(12),
            GuestCount = 2
        };

        var conversationStore = new Mock<IBookingConversationStore>();

        conversationStore
            .Setup(x => x.GetOrCreateAsync(
                conversationId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var aiChatService = new Mock<IAIChatService>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        var availabilityTool = new SearchHouseBoatAvailabilityTool(dbContext);

        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var notificationService = new Mock<INotificationService>();

        var createBookingHandler = new CreateBookingHandler(
            dbContext,
            userContext.Object,
            notificationService.Object);

        var intentValidator = new Mock<IValidator<BookingIntent>>();
        var logger = new Mock<ILogger<BookingAssistantHandler>>();

        var handler = new BookingAssistantHandler(
            aiChatService.Object,
            availabilityTool,
            new SearchAvailabilityRequestValidator(),
            conversationStore.Object,
            createBookingHandler,
            intentValidator.Object,
            logger.Object);

        // Act
        var result = await handler.Handle(
            conversationId,
            "Maybe later",
            CancellationToken.None);

        // Assert
        Assert.Contains("reply 'Yes'", result);

        Assert.True(conversation.IsWaitingForBookingConfirmation);
        Assert.False(conversation.IsBookingCompleted);

        Assert.Equal(0, await dbContext.Bookings.CountAsync());

        aiChatService.Verify(
            x => x.GetStructuredResponseAsync<BookingIntent>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}