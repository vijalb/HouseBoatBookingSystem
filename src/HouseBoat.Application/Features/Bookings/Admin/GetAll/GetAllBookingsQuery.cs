using HouseBoat.Domain.Enums;

namespace HouseBoat.Application.Features.Bookings.Admin.GetAll;

public record GetAllBookingsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    BookingStatus? Status = null,
    Guid? HouseBoatId = null,
    DateTime? CheckInFrom = null,
    DateTime? CheckInTo = null,
    string? SortBy = null,
    string? SortDirection = null);