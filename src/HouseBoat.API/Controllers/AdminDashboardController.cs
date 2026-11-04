using HouseBoat.Application.Features.AdminDashboard.GetDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly GetAdminDashboardHandler _handler;

    public AdminDashboardController(
        GetAdminDashboardHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        CancellationToken cancellationToken)
    {
        var query = new GetAdminDashboardQuery();

        var dashboard = await _handler.Handle(
            query,
            cancellationToken);

        return Ok(dashboard);
    }
}