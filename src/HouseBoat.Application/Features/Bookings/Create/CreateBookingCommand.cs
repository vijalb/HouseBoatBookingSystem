namespace HouseBoat.Application.Features.Bookings.Create;

public record CreateBookingCommand(
    Guid HouseBoatId,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int GuestCount);