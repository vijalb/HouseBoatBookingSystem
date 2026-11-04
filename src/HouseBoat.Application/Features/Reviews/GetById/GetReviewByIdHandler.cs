using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Reviews.GetById;

public class GetReviewByIdHandler
{
    private readonly IApplicationDbContext _context;

    public GetReviewByIdHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewDetailsDto> Handle(
        GetReviewByIdQuery query,
        CancellationToken cancellationToken)
    {
        var review = await _context.Reviews
            .AsNoTracking()
            .Where(x => x.Id == query.ReviewId)
            .Select(x => new ReviewDetailsDto
            {
                Id = x.Id,

                UserId = x.UserId,

                UserEmail = x.User.Email,

                HouseBoatId = x.HouseBoatId,

                HouseBoatName = x.HouseBoat.Name,

                Location = x.HouseBoat.Location,

                Rating = x.Rating,

                Comment = x.Comment,

                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (review is null)
        {
            throw new KeyNotFoundException(
                "Review not found.");
        }

        return review;
    }
}