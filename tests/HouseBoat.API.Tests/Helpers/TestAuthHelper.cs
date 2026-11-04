using HouseBoat.API.Tests.Factories;
using HouseBoat.API.Tests.Helpers;
using HouseBoat.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Text;

namespace HouseBoat.API.Tests.Helpers;

public static class TestAuthHelper
{
    public static string GenerateToken(
        CustomWebApplicationFactory factory,
        string role)
    {
        var jwtOptions = factory.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        var signingKey = (SymmetricSecurityKey)
            jwtOptions.IssuerSigningKey!;

        var key = Encoding.UTF8.GetString(signingKey.Key);

        return JwtTokenHelper.GenerateToken(
            role,
            jwtOptions.ValidIssuer!,
            jwtOptions.ValidAudience!,
            key);
    }

    public static HttpRequestMessage CreateRequest(
        string method,
        string url,
        string token)
    {
        var request = new HttpRequestMessage(
            new HttpMethod(method),
            url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return request;
    }
}