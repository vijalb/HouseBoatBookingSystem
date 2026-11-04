using HouseBoat.Domain.Enums;

namespace HouseBoat.Application.Features.Bookings.GetById;

public class BookingDetailsDto
{
    public Guid Id { get; set; }

    public Guid HouseBoatId { get; set; }

    public string HouseBoatName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public int GuestCount { get; set; }

    public decimal TotalAmount { get; set; }

    public BookingStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}