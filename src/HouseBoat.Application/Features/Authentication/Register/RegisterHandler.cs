using HouseBoat.Application.Exceptions;
using HouseBoat.Application.Interfaces;
using HouseBoat.Domain.Entities;
using HouseBoat.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HouseBoat.Application.Features.Authentication.Register;

public sealed class RegisterHandler
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterHandler(IApplicationDbContext context,IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> HandleAsync(RegisterRequest request,CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(
                x => x.Email == email,
                cancellationToken);

        if (emailExists)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        var passwordHash = _passwordHasher.Hash(
            request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = passwordHash,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Role = UserRole.Customer
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email);
    }
}