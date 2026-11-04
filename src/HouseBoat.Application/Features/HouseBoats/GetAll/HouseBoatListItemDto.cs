namespace HouseBoat.Application.Features.HouseBoats.GetAll;

public record HouseBoatListItemDto(
    Guid Id,
    string Name,
    string Location,
    int Capacity,
    int Bedrooms,
    bool HasAc,
    decimal PricePerNight);