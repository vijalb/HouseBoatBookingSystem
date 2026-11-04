using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.HouseBoats.GetById;

public class GetHouseBoatByIdHandler
{
    private readonly IApplicationDbContext _context;

    public GetHouseBoatByIdHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HouseBoatDetailsDto?> Handle(
        GetHouseBoatByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.Houseboats
            .AsNoTracking()
            .Where(x => x.Id == query.Id)
            .Select(x => new HouseBoatDetailsDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Location = x.Location,
                Capacity = x.Capacity,
                PricePerNight = x.PricePerNight,
                HasAc = x.HasAc,
                Bedrooms = x.Bedrooms,
                IsActive = x.IsActive,

                AverageRating = x.Reviews
        .Select(r => (double?)r.Rating)
        .Average() ?? 0,

                ReviewCount = x.Reviews.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}