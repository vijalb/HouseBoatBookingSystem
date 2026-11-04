using HouseBoat.Application.Features.Reviews.Create;
using HouseBoat.Application.Features.Reviews.Delete;
using HouseBoat.Application.Features.Reviews.GetByHouseBoat;
using HouseBoat.Application.Features.Reviews.GetById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly CreateReviewHandler _createReviewHandler;
    private readonly GetReviewsByHouseBoatHandler _getReviewsByHouseBoatHandler;
    private readonly GetReviewByIdHandler _getReviewByIdHandler;
    private readonly DeleteReviewHandler _deleteReviewHandler;
    public ReviewsController(
        CreateReviewHandler createReviewHandler,
        GetReviewsByHouseBoatHandler getReviewsByHouseBoatHandler,
        GetReviewByIdHandler getReviewByIdHandler,
        DeleteReviewHandler deleteReviewHandler)
    {
        _createReviewHandler = createReviewHandler;
        _getReviewsByHouseBoatHandler = getReviewsByHouseBoatHandler;
        _getReviewByIdHandler = getReviewByIdHandler;
        _deleteReviewHandler = deleteReviewHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var reviewId = await _createReviewHandler.Handle(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(Create),
            new { id = reviewId },
            new
            {
                id = reviewId,
                message = "Review created successfully."
            });
    }

    [HttpGet("houseboat/{houseBoatId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByHouseBoat(
     Guid houseBoatId,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10,
     CancellationToken cancellationToken = default)
    {
        var query = new GetReviewsByHouseBoatQuery(
            houseBoatId,
            pageNumber,
            pageSize);

        var reviews = await _getReviewsByHouseBoatHandler.Handle(
            query,
            cancellationToken);

        return Ok(reviews);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
    Guid id,
    CancellationToken cancellationToken)
    {
        var query = new GetReviewByIdQuery(id);

        var review = await _getReviewByIdHandler.Handle(
            query,
            cancellationToken);

        return Ok(review);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
    Guid id,
    CancellationToken cancellationToken)
    {
        var command = new DeleteReviewCommand(id);

        await _deleteReviewHandler.Handle(
            command,
            cancellationToken);

        return NoContent();
    }
}