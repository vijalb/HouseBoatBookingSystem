using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Reviews.Delete;

public class DeleteReviewHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public DeleteReviewHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task Handle(
        DeleteReviewCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var review = await _context.Reviews
            .FirstOrDefaultAsync(
                x =>
                    x.Id == command.ReviewId &&
                    x.UserId == userId,
                cancellationToken);

        if (review is null)
        {
            throw new KeyNotFoundException(
                "Review not found.");
        }

        _context.Reviews.Remove(review);

        await _context.SaveChangesAsync(cancellationToken);
    }
}