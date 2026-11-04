using HouseBoat.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HouseBoat.API.Tests.Factories;

public class TestDatabaseInitializer : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TestDatabaseInitializer(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await TestDatabaseSeeder.SeedAsync(dbContext);
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}