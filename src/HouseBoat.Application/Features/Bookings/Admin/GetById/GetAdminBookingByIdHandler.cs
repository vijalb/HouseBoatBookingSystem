using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Admin.GetById;

public class GetAdminBookingByIdHandler
{
    private readonly IApplicationDbContext _context;

    public GetAdminBookingByIdHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminBookingDetailsDto> Handle(
        GetAdminBookingByIdQuery query,
        CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(x => x.Id == query.BookingId)
            .Select(x => new AdminBookingDetailsDto
            {
                Id = x.Id,

                UserId = x.UserId,
                UserEmail = x.User.Email,

                HouseBoatId = x.HouseBoatId,
                HouseBoatName = x.HouseBoat.Name,
                Location = x.HouseBoat.Location,

                CheckInDate = x.CheckInDate,
                CheckOutDate = x.CheckOutDate,
                GuestCount = x.GuestCount,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (booking is null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        return booking;
    }
}