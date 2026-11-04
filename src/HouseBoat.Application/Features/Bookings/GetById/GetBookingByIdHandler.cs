using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.GetById;

public class GetBookingByIdHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetBookingByIdHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<BookingDetailsDto> Handle(
        GetBookingByIdQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var booking = await _context.Bookings
            .AsNoTracking()
            .Where(x =>
                x.Id == query.BookingId &&
                x.UserId == userId)
            .Select(x => new BookingDetailsDto
            {
                Id = x.Id,
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