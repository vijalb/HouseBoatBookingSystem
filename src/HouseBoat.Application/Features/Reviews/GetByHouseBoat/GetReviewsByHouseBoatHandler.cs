using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Reviews.GetByHouseBoat;

public class GetReviewsByHouseBoatHandler
{
    private readonly IApplicationDbContext _context;

    public GetReviewsByHouseBoatHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedReviewsDto> Handle(
        GetReviewsByHouseBoatQuery query,
        CancellationToken cancellationToken)
    {
        var houseBoatExists = await _context.Houseboats
            .AnyAsync(
                x => x.Id == query.HouseBoatId,
                cancellationToken);

        if (!houseBoatExists)
        {
            throw new KeyNotFoundException(
                "House boat not found.");
        }

        var totalCount = await _context.Reviews
            .CountAsync(
                x => x.HouseBoatId == query.HouseBoatId,
                cancellationToken);

        var reviews = await _context.Reviews
            .AsNoTracking()
            .Where(x => x.HouseBoatId == query.HouseBoatId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new ReviewDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserEmail = x.User.Email,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)query.PageSize);

        return new PaginatedReviewsDto
        {
            Items = reviews,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}