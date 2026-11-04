using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Admin.Confirm;

public class ConfirmBookingHandler
{
    private readonly IApplicationDbContext _context;

    public ConfirmBookingHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        ConfirmBookingCommand command,
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

        if (booking.Status == BookingStatus.Confirmed)
        {
            throw new InvalidOperationException(
                "Booking is already confirmed.");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Cancelled bookings cannot be confirmed.");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException(
                "Completed bookings cannot be confirmed.");
        }

        booking.Status = BookingStatus.Confirmed;
        booking.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}