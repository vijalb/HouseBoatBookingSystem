using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.HouseBoats.Admin.Deactivate;

public class DeactivateHouseBoatHandler
{
    private readonly IApplicationDbContext _context;

    public DeactivateHouseBoatHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeactivateHouseBoatCommand command,
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

        if (!houseBoat.IsActive)
        {
            throw new InvalidOperationException(
                "House boat is already deactivated.");
        }

        houseBoat.IsActive = false;
        houseBoat.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}