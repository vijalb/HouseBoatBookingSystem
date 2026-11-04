using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;

namespace HouseBoat.Application.Features.HouseBoats.Create;

public class CreateHouseBoatHandler
{
    private readonly IApplicationDbContext _context;

    public CreateHouseBoatHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateHouseBoatCommand command,
        CancellationToken cancellationToken)
    {
        var houseBoat = new Houseboat
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Location = command.Location,
            Capacity = command.Capacity,
            PricePerNight = command.PricePerNight,
            HasAc = command.HasAc,
            Bedrooms = command.Bedrooms,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Houseboats.Add(houseBoat);

        await _context.SaveChangesAsync(
            cancellationToken);

        return houseBoat.Id;
    }
}