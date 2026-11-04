namespace HouseBoat.Application.Features.HouseBoats.Admin.Update;

public record UpdateHouseBoatCommand(
    Guid HouseBoatId,
    string Name,
    string Description,
    string Location,
    int Capacity,
    decimal PricePerNight,
    bool HasAc,
    int Bedrooms);