namespace HouseBoat.Application.Features.HouseBoats.Admin.Create;

public record CreateHouseBoatCommand(
    string Name,
    string Description,
    string Location,
    int Capacity,
    decimal PricePerNight,
    bool HasAc,
    int Bedrooms);