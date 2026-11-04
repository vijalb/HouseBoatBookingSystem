using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Bookings.Create;

public class CreateBookingHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;
    private readonly INotificationService _notificationService;

    public CreateBookingHandler(
        IApplicationDbContext context,
        IUserContext userContext,
        INotificationService notificationService)
    {
        _context = context;
        _userContext = userContext;
        _notificationService= notificationService;
    }

    public async Task<Guid> Handle(
    CreateBookingCommand command,
    CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var checkInDate = command.CheckInDate.Date;
        var checkOutDate = command.CheckOutDate.Date;

        var houseBoat = await _context.Houseboats
            .FirstOrDefaultAsync(
                x => x.Id == command.HouseBoatId,
                cancellationToken);

        if (houseBoat is null)
            throw new KeyNotFoundException("House boat not found.");

        if (!houseBoat.IsActive)
            throw new InvalidOperationException(
                "This house boat is currently unavailable.");

        if (command.GuestCount > houseBoat.Capacity)
            throw new InvalidOperationException(
                $"Guest count cannot exceed the house boat capacity of {houseBoat.Capacity}.");

        var hasOverlappingBooking =
            await _context.Bookings.AnyAsync(
                x =>
                    x.HouseBoatId == command.HouseBoatId
                    &&
                    x.Status != BookingStatus.Cancelled
                    &&
                    x.CheckInDate < checkOutDate
                    &&
                    x.CheckOutDate > checkInDate,
                cancellationToken);

        if (hasOverlappingBooking)
            throw new InvalidOperationException(
                "The house boat is already booked for the selected dates.");

        var numberOfNights =
            (checkOutDate - checkInDate).Days;

        var totalAmount =
            numberOfNights * houseBoat.PricePerNight;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HouseBoatId = command.HouseBoatId,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            GuestCount = command.GuestCount,
            TotalAmount = totalAmount,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Bookings.Add(booking);

        await _context.SaveChangesAsync(cancellationToken);

        await _notificationService.CreateAsync(
            userId,
            "Booking Created",
            $"Your booking for {houseBoat.Name} has been created successfully.",
            cancellationToken);

        return booking.Id;
    }
}