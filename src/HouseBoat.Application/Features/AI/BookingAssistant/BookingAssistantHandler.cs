using FluentValidation;
using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Features.AI.BookingAssistant.Tools;
using HouseBoat.Application.Features.Bookings.Create;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace HouseBoat.Application.Features.AI.BookingAssistant;

public class BookingAssistantHandler
{
    private readonly IAIChatService _aiChatService;
    private readonly SearchHouseBoatAvailabilityTool _availabilityTool;
    private readonly SearchAvailabilityRequestValidator _validator;
    private readonly IBookingConversationStore _conversationStore;
    private readonly CreateBookingHandler _createBookingHandler;
    private readonly IValidator<BookingIntent> _intentValidator;
    private readonly ILogger<BookingAssistantHandler> _logger;
    public BookingAssistantHandler(
    IAIChatService aiChatService,
    SearchHouseBoatAvailabilityTool availabilityTool,
    SearchAvailabilityRequestValidator validator,
    IBookingConversationStore conversationStore,
    CreateBookingHandler createBookingHandler,
    IValidator<BookingIntent> intentValidator,
    ILogger<BookingAssistantHandler> logger)
    {
        _aiChatService = aiChatService;
        _availabilityTool = availabilityTool;
        _validator = validator;
        _conversationStore = conversationStore;
        _createBookingHandler = createBookingHandler;
        _intentValidator = intentValidator;
        _logger = logger;
    }

    public async Task<string> Handle(
        Guid conversationId,
        string userMessage,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("🔥 BOOKING HANDLER HIT");
        var conversation =
    await _conversationStore.GetOrCreateAsync(
        conversationId,
        cancellationToken);
        if (conversation.IsBookingCompleted
        && conversation.CreatedBookingId.HasValue)
        {
            return $"""
        This conversation has already created a booking.

        Booking ID: {conversation.CreatedBookingId}
        Please use the booking management options if you need to manage it.
        """;
        }
        // Handle booking confirmation

        // Handle booking confirmation
        if (conversation.IsWaitingForBookingConfirmation)
        {
            // Normalize the user's reply so punctuation and casing don't matter.
            var normalizedMessage = userMessage
                .Trim()
                .TrimEnd('.', '!', '?')
                .ToLowerInvariant();

            var isConfirmation =
                normalizedMessage == "yes" ||
                normalizedMessage == "y" ||
                normalizedMessage == "yes, proceed" ||
                normalizedMessage == "yes, please book it" ||
                normalizedMessage == "please book it" ||
                normalizedMessage == "confirm";

            var isCancellation =
                normalizedMessage == "no" ||
                normalizedMessage == "n" ||
                normalizedMessage == "cancel" ||
                normalizedMessage == "no, cancel";

            // User confirms the booking
            if (isConfirmation)
            {
                if (conversation.HouseBoatName is null ||
                    conversation.CheckInDate is null ||
                    conversation.CheckOutDate is null ||
                    conversation.GuestCount is null)
                {
                    conversation.IsWaitingForBookingConfirmation = false;

                    await _conversationStore.SaveAsync(
                        conversation,
                        cancellationToken);

                    return "Your booking details are incomplete. Please start the booking request again.";
                }

                var request = new SearchAvailabilityRequest(
                    conversation.Location,
                    conversation.CheckInDate.Value,
                    conversation.CheckOutDate.Value,
                    conversation.GuestCount.Value);

                var validationResult = await _validator.ValidateAsync(
                    request,
                    cancellationToken);

                if (!validationResult.IsValid)
                {
                    conversation.IsWaitingForBookingConfirmation = false;

                    await _conversationStore.SaveAsync(
                        conversation,
                        cancellationToken);

                    return string.Join(
                        "\n",
                        validationResult.Errors.Select(x => x.ErrorMessage));
                }

                var houseBoat = (await _availabilityTool.ExecuteAsync(
                        request,
                        cancellationToken))
                    .FirstOrDefault(x =>
                        x.Name.Equals(
                            conversation.HouseBoatName,
                            StringComparison.OrdinalIgnoreCase));

                if (houseBoat is null)
                {
                    conversation.IsWaitingForBookingConfirmation = false;
                    conversation.IsBookingCompleted = false;
                    conversation.CreatedBookingId = null;

                    await _conversationStore.SaveAsync(
                        conversation,
                        cancellationToken);

                    return "Sorry, that houseboat is not available for your selected dates. Please search again.";
                }

                var command = new CreateBookingCommand(
                    houseBoat.Id,
                    conversation.CheckInDate.Value,
                    conversation.CheckOutDate.Value,
                    conversation.GuestCount.Value);

                Guid bookingId;

                try
                {
                    bookingId = await _createBookingHandler.Handle(
                        command,
                        cancellationToken);
                }
                catch (InvalidOperationException ex)
                {
                    conversation.IsWaitingForBookingConfirmation = false;
                    conversation.IsBookingCompleted = false;
                    conversation.CreatedBookingId = null;

                    await _conversationStore.SaveAsync(
                        conversation,
                        cancellationToken);

                    return $"""
                    Sorry, we couldn't complete your booking.

                    Reason: {ex.Message}

                    Please check availability and try again.
                    """;
                }

                conversation.IsWaitingForBookingConfirmation = false;
                conversation.IsBookingCompleted = true;
                conversation.CreatedBookingId = bookingId;

                await _conversationStore.SaveAsync(
                    conversation,
                    cancellationToken);

                return $"""
                Your booking request has been created successfully!

                Houseboat: {conversation.HouseBoatName}
                Booking ID: {bookingId}
                Status: Pending
                """;
            }

            // User cancels the booking
            if (isCancellation)
            {
                conversation.IsWaitingForBookingConfirmation = false;

                await _conversationStore.SaveAsync(
                    conversation,
                    cancellationToken);

                return "Booking request cancelled. No booking has been created.";
            }

            // User's response wasn't recognized as confirmation or cancellation.
            return "Please reply 'Yes' to proceed with the booking or 'No' to cancel.";
        }

        var currentDate = DateTime.UtcNow.Date;

        // Step 1: Ask AI to understand the user's booking request
        var extractionPrompt = $"""
                                You are a booking assistant for a houseboat booking system.

                                Today's date is: {currentDate:yyyy-MM-dd}.

                                Existing booking information:
                                Location: {conversation.Location ?? "null"}
                                HouseBoatName: {conversation.HouseBoatName ?? "null"}
                                CheckInDate: {conversation.CheckInDate?.ToString("yyyy-MM-dd") ?? "null"}
                                CheckOutDate: {conversation.CheckOutDate?.ToString("yyyy-MM-dd") ?? "null"}
                                GuestCount: {conversation.GuestCount?.ToString() ?? "null"}

                                Extract information from the customer's latest message.

                                Return:
                                - Intent
                                - Location
                                - HouseBoatName
                                - CheckInDate
                                - CheckOutDate
                                - GuestCount

                                Rules:
                                - Use the existing booking information as context.
                                - Only change a field when the customer provides new information.
                                - If the customer does not mention a field, return null for that field.
                                - If the user is asking about available houseboats,
                                  Intent should be "SearchAvailability".
                                - If the user wants to book/reserve a specific houseboat,
                                  Intent should be "BookHouseBoat".
                                - If the user wants to cancel a booking,
                                  Intent should be "CancelBooking".
                                - Otherwise, use "General".
                                - Dates without a year should use the next occurrence
                                  relative to today's date.
                                - Do not invent missing information.

                                Customer message:
                                {userMessage}
                                """;

        BookingIntent intent;

        try
        {
            intent = await _aiChatService
                .GetStructuredResponseAsync<BookingIntent>(
                    extractionPrompt,
                    cancellationToken);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI intent extraction failed for conversation {ConversationId}",
                conversationId);

            return "Sorry, the booking assistant is temporarily unavailable. " +
                   "Please try again in a moment.";
        }
        if (intent is null)
        {
            _logger.LogWarning(
                "AI returned an empty booking intent for conversation {ConversationId}",
                conversationId);

            return "I couldn't understand your request. " +
                   "Please try rephrasing your booking requirements.";
        }
        var validationResults = await _intentValidator
                               .ValidateAsync(intent, cancellationToken);

        if (!validationResults.IsValid)
        {
            return "I couldn't understand your booking request properly. " +
                   "Could you please provide your requirements again?";
        }
        var message = userMessage.Trim().ToLowerInvariant();

        var explicitlyWantsBooking =
            message.Contains("book") ||
            message.Contains("reserve");

        var asksForAvailability =
            !explicitlyWantsBooking &&
            (
                message.Contains("houseboat") &&
                (
                    message.Contains("need") ||
                    message.Contains("find") ||
                    message.Contains("looking for") ||
                    message.Contains("available") ||
                    message.Contains("availability") ||
                    message.Contains("search")
                )
            );

        if (asksForAvailability)
        {
            intent = intent with { Intent = "SearchAvailability" };
        }

        conversation.Location ??= intent.Location;
        if (intent.HouseBoatName is not null)
            conversation.HouseBoatName = intent.HouseBoatName;
        conversation.CheckInDate ??= intent.CheckInDate;
        conversation.CheckOutDate ??= intent.CheckOutDate;
        conversation.GuestCount ??= intent.GuestCount;

        await _conversationStore.SaveAsync(
        conversation,
        cancellationToken);
        Console.WriteLine(
                        $"CONVERSATION STATE -> Location={conversation.Location}, " +
                        $"CheckIn={conversation.CheckInDate:yyyy-MM-dd}, " +
                        $"CheckOut={conversation.CheckOutDate:yyyy-MM-dd}, " +
                        $"Guests={conversation.GuestCount}");

        Console.WriteLine($"DETECTED INTENT -> {intent.Intent}");

        if (intent.Intent.Equals(
                "BookHouseBoat",
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                $"EXTRACTED BOOKING -> " +
                $"Intent={intent.Intent}, " +
                $"HouseBoatName={intent.HouseBoatName}");

            if (string.IsNullOrWhiteSpace(conversation.HouseBoatName))
            {
                return "Which houseboat would you like to book?";
            }

            if (conversation.CheckInDate is null ||
                conversation.CheckOutDate is null ||
                conversation.GuestCount is null)
            {
                var missingInformation = new List<string>();

                if (conversation.CheckInDate is null)
                    missingInformation.Add("check-in date");

                if (conversation.CheckOutDate is null)
                    missingInformation.Add("check-out date");

                if (conversation.GuestCount is null)
                    missingInformation.Add("number of guests");

                return $"Please provide your {string.Join(", ", missingInformation)}.";
            }

            conversation.IsWaitingForBookingConfirmation = true;
            await _conversationStore.SaveAsync(
            conversation,
            cancellationToken);

            return $"""
                Please review your booking request:

                Houseboat: {conversation.HouseBoatName}
                Check-in: {conversation.CheckInDate:yyyy-MM-dd}
                Check-out: {conversation.CheckOutDate:yyyy-MM-dd}
                Guests: {conversation.GuestCount}

                Would you like to proceed with this booking?
                """;
        }

        // Step 2: Check whether this is an availability request
        if (intent.Intent.Equals(
        "SearchAvailability",
        StringComparison.OrdinalIgnoreCase)
        || (
            conversation.CheckInDate is not null &&
            conversation.CheckOutDate is not null &&
            conversation.GuestCount is not null &&
            !string.IsNullOrWhiteSpace(conversation.Location) &&
            !intent.Intent.Equals(
                "CancelBooking",
                StringComparison.OrdinalIgnoreCase)
        ))
        {
            // Step 3: Make sure required information exists
            if (conversation.CheckInDate is null ||
                conversation.CheckOutDate is null ||
                conversation.GuestCount is null)
            {
                var missingInformation = new List<string>();

                if (conversation.CheckInDate is null)
                    missingInformation.Add("check-in date");

                if (conversation.CheckOutDate is null)
                    missingInformation.Add("check-out date");

                if (conversation.GuestCount is null)
                    missingInformation.Add("number of guests");

                return $"Please provide your {string.Join(", ", missingInformation)}.";
            }

            // Step 4: Create the request for our application tool
            var request = new SearchAvailabilityRequest(
                        conversation.Location,
                        conversation.CheckInDate.Value,
                        conversation.CheckOutDate.Value,
                        conversation.GuestCount.Value);

                                Console.WriteLine(
                        $"AI INTENT@@@@@@@@@@@@@ -> Intent={intent.Intent}, " +
                        $"Location@@@@@@@@@@@@@@@@@@@@={intent.Location}, " +
                        $"CheckIn$$$$$$$$$$$$={intent.CheckInDate:yyyy-MM-dd}, " +
                        $"CheckOut!!!!!!!!!!={intent.CheckOutDate:yyyy-MM-dd}, " +
                        $"Guests^^^^^^^^^={intent.GuestCount}");

            var validationResult = await _validator.ValidateAsync(
                                    request,
                                    cancellationToken);

            if (!validationResult.IsValid)
            {
                return string.Join(
                    "\n",
                    validationResult.Errors.Select(x => x.ErrorMessage));
            }

            // Step 5: C# directly calls the availability tool
            var availableHouseBoats =
                await _availabilityTool.ExecuteAsync(
                    request,
                    cancellationToken);

            // Step 6: Give the actual database results back to AI
            var resultPrompt = $"""
                You are a helpful houseboat booking assistant.

                The customer asked:
                {userMessage}

                The system searched the database and returned these
                available houseboats:

                {System.Text.Json.JsonSerializer.Serialize(availableHouseBoats)}

                Explain the results clearly to the customer.

                Do not invent houseboats, prices, locations,
                availability, or other information.

                If no houseboats are available, simply tell the customer
                that no available houseboats were found for the requested
                criteria.
                """;


            


            return await _aiChatService.GetResponseAsync(
                resultPrompt,
                cancellationToken);
        }

        // Other intents will be handled later
        return await _aiChatService.GetResponseAsync(
            userMessage,
            cancellationToken);
    }
}