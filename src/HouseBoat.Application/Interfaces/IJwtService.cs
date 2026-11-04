using HouseBoat.Domain.Enums;

namespace HouseBoat.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        Guid userId,
        string email,
        UserRole role);
}