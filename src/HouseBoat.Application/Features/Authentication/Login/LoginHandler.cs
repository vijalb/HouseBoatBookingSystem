using HouseBoat.Application.Exceptions;
using HouseBoat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Authentication.Login;

public sealed class LoginHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtService jwtService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> HandleAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException(
                "This account is inactive.");
        }

        var passwordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var accessToken = _jwtService.GenerateToken(
    user.Id,
    user.Email,
    user.Role);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(60);

        return new LoginResponse(
            accessToken,
            expiresAtUtc);
    }
}