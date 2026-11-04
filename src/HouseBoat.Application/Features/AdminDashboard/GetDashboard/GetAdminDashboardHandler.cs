using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.AdminDashboard.GetDashboard;

public class GetAdminDashboardHandler
{
    private readonly IApplicationDbContext _context;

    public GetAdminDashboardHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardDto> Handle(
        GetAdminDashboardQuery query,
        CancellationToken cancellationToken)
    {
        var totalHouseBoats = await _context.Houseboats
            .CountAsync(cancellationToken);

        var activeHouseBoats = await _context.Houseboats
            .CountAsync(
                x => x.IsActive,
                cancellationToken);

        var totalBookings = await _context.Bookings
            .CountAsync(cancellationToken);

        var pendingBookings = await _context.Bookings
            .CountAsync(
                x => x.Status == BookingStatus.Pending,
                cancellationToken);

        var confirmedBookings = await _context.Bookings
            .CountAsync(
                x => x.Status == BookingStatus.Confirmed,
                cancellationToken);

        var completedBookings = await _context.Bookings
            .CountAsync(
                x => x.Status == BookingStatus.Completed,
                cancellationToken);

        var cancelledBookings = await _context.Bookings
            .CountAsync(
                x => x.Status == BookingStatus.Cancelled,
                cancellationToken);

        var totalReviews = await _context.Reviews
            .CountAsync(cancellationToken);

        var averageRating = await _context.Reviews
            .Select(x => (double?)x.Rating)
            .AverageAsync(cancellationToken) ?? 0;

        return new AdminDashboardDto
        {
            TotalHouseBoats = totalHouseBoats,
            ActiveHouseBoats = activeHouseBoats,
            InactiveHouseBoats =
                totalHouseBoats - activeHouseBoats,

            TotalBookings = totalBookings,
            PendingBookings = pendingBookings,
            ConfirmedBookings = confirmedBookings,
            CompletedBookings = completedBookings,
            CancelledBookings = cancelledBookings,

            TotalReviews = totalReviews,
            AverageRating = Math.Round(
                averageRating,
                2)
        };
    }
}