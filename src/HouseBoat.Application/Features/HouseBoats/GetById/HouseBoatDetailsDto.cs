namespace HouseBoat.Application.Features.HouseBoats.GetById;

public class HouseBoatDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public decimal PricePerNight { get; set; }

    public bool HasAc { get; set; }

    public DateTime CreatedAt { get; set; }

    public int Bedrooms { get; set; }

    public bool IsActive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public double AverageRating { get; set; }

    public int ReviewCount { get; set; }
}