namespace HouseBoat.Application.Features.Reviews.GetByHouseBoat;

public record GetReviewsByHouseBoatQuery(
    Guid HouseBoatId,
    int PageNumber = 1,
    int PageSize = 10);