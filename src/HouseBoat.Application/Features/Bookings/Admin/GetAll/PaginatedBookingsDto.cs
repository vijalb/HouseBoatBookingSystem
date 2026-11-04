namespace HouseBoat.Application.Features.Bookings.Admin.GetAll;

public class PaginatedBookingsDto
{
    public List<AdminBookingDto> Items { get; set; } = new();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}