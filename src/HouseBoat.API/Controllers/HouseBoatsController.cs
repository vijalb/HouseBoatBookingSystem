using HouseBoat.Application.Features.HouseBoats.Admin.Activate;
using HouseBoat.Application.Features.HouseBoats.Admin.Deactivate;
using HouseBoat.Application.Features.HouseBoats.Admin.Update;
using HouseBoat.Application.Features.HouseBoats.Create;
using HouseBoat.Application.Features.HouseBoats.GetAll;
using HouseBoat.Application.Features.HouseBoats.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseBoatsController : ControllerBase
{
    private readonly CreateHouseBoatHandler _createHandler;
    private readonly GetHouseBoatsHandler _getHandler;
    private readonly GetHouseBoatByIdHandler _getByIdHandler;
    private readonly CreateHouseBoatHandler _createHouseBoatHandler;
    private readonly UpdateHouseBoatHandler _updateHouseBoatHandler;
    private readonly DeactivateHouseBoatHandler _deactivateHouseBoatHandler;
    private readonly ActivateHouseBoatHandler _activateHouseBoatHandler;
    public HouseBoatsController(
        CreateHouseBoatHandler createHandler,GetHouseBoatsHandler getHandler,GetHouseBoatByIdHandler getByIdHandler,
        CreateHouseBoatHandler createHouseBoatHandler,
        UpdateHouseBoatHandler updateHouseBoatHandler,
        DeactivateHouseBoatHandler deactivateHouseBoatHandler,
        ActivateHouseBoatHandler activateHouseBoatHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _getByIdHandler = getByIdHandler;
        _createHouseBoatHandler = createHouseBoatHandler;
        _updateHouseBoatHandler = updateHouseBoatHandler;
        _deactivateHouseBoatHandler = deactivateHouseBoatHandler;
        _activateHouseBoatHandler = activateHouseBoatHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateHouseBoatCommand command,CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(Create),
            new { id },
            new
            {
                id
            });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var result = await _getHandler.Handle(
            new GetHouseBoatsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _getByIdHandler.Handle(
            new GetHouseBoatByIdQuery(id),
            cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Createhb(
    [FromBody] CreateHouseBoatCommand command,
    CancellationToken cancellationToken)
    {
        var houseBoatId =
            await _createHouseBoatHandler.Handle(
                command,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = houseBoatId },
            new
            {
                id = houseBoatId,
                message = "House boat created successfully."
            });
    }

    [HttpPut("admin/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateHouseBoatCommand command,
    CancellationToken cancellationToken)
    {
        if (id != command.HouseBoatId)
        {
            return BadRequest(new
            {
                message = "Route ID and request ID do not match."
            });
        }

        await _updateHouseBoatHandler.Handle(
            command,
            cancellationToken);

        return Ok(new
        {
            message = "House boat updated successfully."
        });
    }

    [HttpPut("admin/{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _deactivateHouseBoatHandler.Handle(
            new DeactivateHouseBoatCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "House boat deactivated successfully."
        });
    }

    [HttpPut("admin/{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(
    Guid id,
    CancellationToken cancellationToken)
    {
        await _activateHouseBoatHandler.Handle(
            new ActivateHouseBoatCommand(id),
            cancellationToken);

        return Ok(new
        {
            message = "House boat activated successfully."
        });
    }
}