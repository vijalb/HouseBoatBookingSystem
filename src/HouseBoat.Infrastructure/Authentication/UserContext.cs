using System.Security.Claims;
using HouseBoat.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HouseBoat.Infrastructure.Authentication;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            if (!Guid.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user identifier.");
            }

            return id;
        }
    }
}