using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.AI.BookingAssistant.Tools;

public class SearchHouseBoatAvailabilityTool
{
    private readonly IApplicationDbContext _context;

    public SearchHouseBoatAvailabilityTool(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailableHouseBoatDto>> ExecuteAsync(
        SearchAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
        $"AI TOOL INPUT -> Location={request.Location}, " +
        $"CheckIn={request.CheckInDate:yyyy-MM-dd}, " +
        $"CheckOut={request.CheckOutDate:yyyy-MM-dd}, " +
        $"Guests={request.GuestCount}");

        var checkInDate = request.CheckInDate.Date;
        var checkOutDate = request.CheckOutDate.Date;

        var query = _context.Houseboats
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                x.Capacity >= request.GuestCount);

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            query = query.Where(x =>
                x.Location.Contains(request.Location));
        }

        var availableHouseBoats = await query
            .Where(houseBoat =>
                !_context.Bookings.Any(booking =>
                    booking.HouseBoatId == houseBoat.Id &&
                    (booking.Status == BookingStatus.Pending ||
                     booking.Status == BookingStatus.Confirmed) &&
                    booking.CheckInDate < checkOutDate &&
                    booking.CheckOutDate > checkInDate))
            .Select(x => new AvailableHouseBoatDto(
                x.Id,
                x.Name,
                x.Location,
                x.Capacity,
                x.PricePerNight,
                x.Bedrooms,
                x.HasAc))
            .ToListAsync(cancellationToken);

        return availableHouseBoats;
    }
}