namespace HouseBoat.Application.Features.AI.BookingAssistant.Tools;

public record AvailableHouseBoatDto(
    Guid Id,
    string Name,
    string Location,
    int Capacity,
    decimal PricePerNight,
    int Bedrooms,
    bool HasAc);