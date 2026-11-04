namespace HouseBoat.Application.Features.HouseBoats.Create;

public record CreateHouseBoatCommand(
    string Name,
    string Description,
    string Location,
    int Capacity,
    decimal PricePerNight,
    bool HasAc,
    int Bedrooms);