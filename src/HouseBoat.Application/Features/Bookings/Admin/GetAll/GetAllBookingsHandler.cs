using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Admin.GetAll;

public class GetAllBookingsHandler
{
    private readonly IApplicationDbContext _context;

    public GetAllBookingsHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedBookingsDto> Handle(
        GetAllBookingsQuery query,
        CancellationToken cancellationToken)
    {
        var bookingsQuery = _context.Bookings
            .AsNoTracking()
            .AsQueryable();

        // Filtering
        if (query.Status.HasValue)
        {
            bookingsQuery = bookingsQuery.Where(
                x => x.Status == query.Status.Value);
        }

        if (query.HouseBoatId.HasValue)
        {
            bookingsQuery = bookingsQuery.Where(
                x => x.HouseBoatId == query.HouseBoatId.Value);
        }

        if (query.CheckInFrom.HasValue)
        {
            var fromDate = query.CheckInFrom.Value.Date;

            bookingsQuery = bookingsQuery.Where(
                x => x.CheckInDate >= fromDate);
        }

        if (query.CheckInTo.HasValue)
        {
            var toDate = query.CheckInTo.Value.Date;

            bookingsQuery = bookingsQuery.Where(
                x => x.CheckInDate <= toDate);
        }

        // Count after filtering
        var totalCount = await bookingsQuery
            .CountAsync(cancellationToken);

        // Sorting
        var sortBy = query.SortBy?.Trim().ToLowerInvariant();
        var sortDirection = query.SortDirection?.Trim().ToLowerInvariant();

        var descending = sortDirection == "desc";

        bookingsQuery = sortBy switch
        {
            "checkindate" =>
                descending
                    ? bookingsQuery.OrderByDescending(x => x.CheckInDate)
                    : bookingsQuery.OrderBy(x => x.CheckInDate),

            "checkoutdate" =>
                descending
                    ? bookingsQuery.OrderByDescending(x => x.CheckOutDate)
                    : bookingsQuery.OrderBy(x => x.CheckOutDate),

            "totalamount" =>
                descending
                    ? bookingsQuery.OrderByDescending(x => x.TotalAmount)
                    : bookingsQuery.OrderBy(x => x.TotalAmount),

            "guestcount" =>
                descending
                    ? bookingsQuery.OrderByDescending(x => x.GuestCount)
                    : bookingsQuery.OrderBy(x => x.GuestCount),

            _ =>
                descending
                    ? bookingsQuery.OrderByDescending(x => x.CreatedAt)
                    : bookingsQuery.OrderBy(x => x.CreatedAt)
        };

        // Pagination
        var bookings = await bookingsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new AdminBookingDto
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
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)query.PageSize);

        return new PaginatedBookingsDto
        {
            Items = bookings,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}