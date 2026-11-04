using HouseBoat.Domain.Enums;

namespace HouseBoat.Application.Features.Bookings.Admin.GetById;

public class AdminBookingDetailsDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = string.Empty;

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