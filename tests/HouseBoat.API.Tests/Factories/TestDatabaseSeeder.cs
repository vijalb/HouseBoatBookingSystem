using HouseBoat.Domain.Entities;
using HouseBoat.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.API.Tests.Factories;

public static class TestDatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext)
    {
        var houseBoatId = Guid.Parse(
            "a90f39cc-eb6e-40ed-acc3-e8998d830979");

        var exists = await dbContext.Houseboats
            .AnyAsync(x => x.Id == houseBoatId);

        if (exists)
            return;

        var houseBoat = new HouseBoat.Domain.Entities.Houseboat
        {
            Id = houseBoatId,
            Name = "Kerala Queen",
            Description = "Test houseboat",
            Location = "Alappuzha",
            Capacity = 6,
            PricePerNight = 15000,
            HasAc = true,
            Bedrooms = 3,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Houseboats.Add(houseBoat);

        await dbContext.SaveChangesAsync();
    }
}