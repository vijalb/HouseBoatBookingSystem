namespace HouseBoat.Application.Features.Reviews.GetById;

public class ReviewDetailsDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserEmail { get; set; } = string.Empty;

    public Guid HouseBoatId { get; set; }

    public string HouseBoatName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}