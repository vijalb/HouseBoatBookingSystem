namespace HouseBoat.Application.Features.Reviews.GetByHouseBoat;

public class ReviewDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}