using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Cancel;

public class CancelBookingHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public CancelBookingHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task Handle(
        CancelBookingCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var booking = await _context.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == command.BookingId &&
                     x.UserId == userId,
                cancellationToken);

        if (booking is null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Booking is already cancelled.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed bookings cannot be cancelled.");
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}