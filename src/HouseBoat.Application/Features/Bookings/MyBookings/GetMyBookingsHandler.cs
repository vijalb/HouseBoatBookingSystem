using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.MyBookings;

public class GetMyBookingsHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public GetMyBookingsHandler(
        IApplicationDbContext context,
        IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task<List<MyBookingDto>> Handle(
        GetMyBookingsQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        return await _context.Bookings
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new MyBookingDto
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
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}