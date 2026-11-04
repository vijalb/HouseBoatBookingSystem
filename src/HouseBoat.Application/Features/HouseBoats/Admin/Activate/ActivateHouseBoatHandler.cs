using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.HouseBoats.Admin.Activate;

public class ActivateHouseBoatHandler
{
    private readonly IApplicationDbContext _context;

    public ActivateHouseBoatHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        ActivateHouseBoatCommand command,
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

        if (houseBoat.IsActive)
        {
            throw new InvalidOperationException(
                "House boat is already active.");
        }

        houseBoat.IsActive = true;
        houseBoat.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}