using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Admin.Complete;

public class CompleteBookingHandler
{
    private readonly IApplicationDbContext _context;

    public CompleteBookingHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        CompleteBookingCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == command.BookingId,
                cancellationToken);

        if (booking is null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException(
                "Booking is already completed.");
        }

        if (booking.Status == BookingStatus.Pending)
        {
            throw new InvalidOperationException(
                "Pending bookings cannot be completed.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cancelled bookings cannot be completed.");
        }

        booking.Status = BookingStatus.Completed;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}