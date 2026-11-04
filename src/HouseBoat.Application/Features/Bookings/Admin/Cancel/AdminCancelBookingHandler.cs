using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Admin.Cancel;

public class AdminCancelBookingHandler
{
    private readonly IApplicationDbContext _context;

    public AdminCancelBookingHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        AdminCancelBookingCommand command,
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