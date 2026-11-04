using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Reviews.Create;

public class CreateReviewHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public CreateReviewHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(
        CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        // 1. Check whether the house boat exists
        var houseBoatExists = await _context.Houseboats
            .AnyAsync(
                x => x.Id == command.HouseBoatId,
                cancellationToken);

        if (!houseBoatExists)
        {
            throw new KeyNotFoundException(
                "House boat not found.");
        }

        // 2. Check whether the customer has completed
        //    a booking for this house boat
        var hasCompletedBooking = await _context.Bookings
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.HouseBoatId == command.HouseBoatId &&
                    x.Status == BookingStatus.Completed,
                cancellationToken);

        if (!hasCompletedBooking)
        {
            throw new InvalidOperationException(
                "You can review a house boat only after completing a booking.");
        }

        // 3. Prevent duplicate reviews
        var alreadyReviewed = await _context.Reviews
            .AnyAsync(
                x =>
                    x.UserId == userId &&
                    x.HouseBoatId == command.HouseBoatId,
                cancellationToken);

        if (alreadyReviewed)
        {
            throw new InvalidOperationException(
                "You have already reviewed this house boat.");
        }

        // 4. Create review
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HouseBoatId = command.HouseBoatId,
            Rating = command.Rating,
            Comment = command.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync(cancellationToken);

        return review.Id;
    }
}