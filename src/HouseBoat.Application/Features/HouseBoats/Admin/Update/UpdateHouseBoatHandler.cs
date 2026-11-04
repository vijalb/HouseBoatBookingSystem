using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.HouseBoats.Admin.Update;

public class UpdateHouseBoatHandler
{
    private readonly IApplicationDbContext _context;

    public UpdateHouseBoatHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdateHouseBoatCommand command,
        CancellationToken cancellationToken)
    {
        var houseBoat = await _context.Houseboats
            .FirstOrDefaultAsync(
                x => x.Id == command.HouseBoatId,
                cancellationToken);

        if (houseBoat is null)
        {
            throw new KeyNotFoundException(
                "House boat not found.");
        }

        houseBoat.Name = command.Name.Trim();
        houseBoat.Description = command.Description.Trim();
        houseBoat.Location = command.Location.Trim();
        houseBoat.Capacity = command.Capacity;
        houseBoat.PricePerNight = command.PricePerNight;
        houseBoat.HasAc = command.HasAc;
        houseBoat.Bedrooms = command.Bedrooms;
        houseBoat.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}