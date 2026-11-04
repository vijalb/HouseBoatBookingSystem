namespace HouseBoat.Application.Features.AI.BookingAssistant;

public record BookingIntent(
    string Intent,
    string? Location,
    string? HouseBoatName,
    DateTime? CheckInDate,
    DateTime? CheckOutDate,
    int? GuestCount);