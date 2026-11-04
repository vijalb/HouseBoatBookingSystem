namespace HouseBoat.Application.Features.Reviews.Create;

public record CreateReviewCommand(
    Guid HouseBoatId,
    int Rating,
    string Comment);