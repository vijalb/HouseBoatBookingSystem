using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.HouseBoats.GetAll;

public class GetHouseBoatsHandler
{
    private readonly IApplicationDbContext _context;

    public GetHouseBoatsHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HouseBoatListItemDto>> Handle(
        GetHouseBoatsQuery query,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("========== AI INTENT REACHED ==========");
        return await _context.Houseboats
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new HouseBoatListItemDto(
                x.Id,
                x.Name,
                x.Location,
                x.Capacity,
                x.Bedrooms,
                x.HasAc,
                x.PricePerNight))
            .ToListAsync(cancellationToken);
    }
}