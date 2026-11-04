namespace HouseBoat.Application.Features.AI.BookingAssistant.Tools;

public record SearchAvailabilityRequest(
    string? Location,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int GuestCount);