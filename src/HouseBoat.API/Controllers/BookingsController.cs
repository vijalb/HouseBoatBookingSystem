using HouseBoat.Application.Features.Bookings.Admin.Cancel;
using HouseBoat.Application.Features.Bookings.Admin.Complete;
using HouseBoat.Application.Features.Bookings.Admin.Confirm;
using HouseBoat.Application.Features.Bookings.Admin.GetAll;
using HouseBoat.Application.Features.Bookings.Admin.GetById;
using HouseBoat.Application.Features.Bookings.Cancel;
using HouseBoat.Application.Features.Bookings.Create;
using HouseBoat.Application.Features.Bookings.GetById;
using HouseBoat.Application.Features.Bookings.MyBookings;
using HouseBoat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly CreateBookingHandler _createBookingHandler;
    private readonly GetMyBookingsHandler _getMyBookingsHandler;
    private readonly GetBookingByIdHandler _getBookingByIdHandler;
    private readonly CancelBookingHandler _cancelBookingHandler;
    private readonly GetAllBookingsHandler _getAllBookingsHandler;
    private readonly GetAdminBookingByIdHandler _getAdminBookingByIdHandler;
    private readonly ConfirmBookingHandler _confirmBookingHandler;
    private readonly AdminCancelBookingHandler _adminCancelBookingHandler;
    private readonly CompleteBookingHandler _completeBookingHandler;
    public BookingsController(
    CreateBookingHandler createBookingHandler,
    GetMyBookingsHandler getMyBookingsHandler,
    GetBookingByIdHandler getBookingByIdHandler,
    CancelBookingHandler cancelBookingHandler,
    GetAllBookingsHandler getAllBookingsHandler,
    GetAdminBookingByIdHandler getAdminBookingByIdHandler,
    ConfirmBookingHandler confirmBookingHandler,
    AdminCancelBookingHandler adminCancelBookingHandler,
    CompleteBookingHandler completeBookingHandler)
    {
        _createBookingHandler = createBookingHandler;
        _getMyBookingsHandler = getMyBookingsHandler;
        _getBookingByIdHandler = getBookingByIdHandler;
        _cancelBookingHandler = cancelBookingHandler;
        _getAllBookingsHandler = getAllBookingsHandler;
        _getAdminBookingByIdHandler = getAdminBookingByIdHandler;
        _confirmBookingHandler = confirmBookingHandler;
        _adminCancelBookingHandler = adminCancelBookingHandler;
        _completeBookingHandler = completeBookingHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var bookingId = await _createBookingHandler.Handle(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(Create),
            new { id = bookingId },
            new
            {
                id = bookingId,
                message = "Booking created successfully."
            });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings(
        CancellationToken cancellationToken)
    {
        var bookings = await _getMyBookingsHandler.Handle(
            new GetMyBookingsQuery(),
            cancellationToken);

        return Ok(bookings);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var booking = await _getBookingByIdHandler.Handle(
            new GetBookingByIdQuery(id),
            cancellationToken);

        return Ok(booking);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _cancelBookingHandler.Handle(
            new CancelBookingCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "Booking cancelled successfully."
        });
    }

    [HttpGet("admin-test")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminTest()
    {
        return Ok(new
        {
            message = "You have access to the Admin area."
        });
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] BookingStatus? status = null,
        [FromQuery] Guid? houseBoatId = null,
        [FromQuery] DateTime? checkInFrom = null,
        [FromQuery] DateTime? checkInTo = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllBookingsQuery(
            pageNumber,
            pageSize,
            status,
            houseBoatId,
            checkInFrom,
            checkInTo,
            sortBy,
            sortDirection);

        var bookings = await _getAllBookingsHandler.Handle(
            query,
            cancellationToken);

        return Ok(bookings);
    }

    [HttpGet("admin/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminBookingById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var booking = await _getAdminBookingByIdHandler.Handle(
            new GetAdminBookingByIdQuery(id),
            cancellationToken);

        return Ok(booking);
    }

    [HttpPut("admin/{id:guid}/confirm")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Confirm(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _confirmBookingHandler.Handle(
            new ConfirmBookingCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "Booking confirmed successfully."
        });
    }

    [HttpPut("admin/{id:guid}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCancel(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _adminCancelBookingHandler.Handle(
            new AdminCancelBookingCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "Booking cancelled successfully."
        });
    }

    [HttpPut("admin/{id:guid}/complete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Complete(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _completeBookingHandler.Handle(
            new CompleteBookingCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "Booking completed successfully."
        });
    }
}