namespace HouseBoat.Application.Features.Reviews.GetByHouseBoat;

public class PaginatedReviewsDto
{
    public List<ReviewDto> Items { get; set; } = new();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}