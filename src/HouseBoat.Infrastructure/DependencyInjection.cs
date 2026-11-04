using HouseBoat.Application.Common.Interfaces;
using HouseBoat.Application.Interfaces;
using HouseBoat.Infrastructure.AI.Ollama;
using HouseBoat.Infrastructure.AI.OpenAI;
using HouseBoat.Infrastructure.Authentication;
using HouseBoat.Infrastructure.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HouseBoat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
        configuration.GetSection(JwtOptions.SectionName));
        //    services.Configure<OpenAIOptions>(
        //configuration.GetSection(OpenAIOptions.SectionName));
        services.Configure<OllamaOptions>(
        configuration.GetSection(OllamaOptions.SectionName));
        services.AddScoped<IAIChatService, OllamaChatService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<INotificationService,NotificationService>();
        //services.AddScoped<IAIChatService, OpenAIChatService>();
        return services;
    }
}