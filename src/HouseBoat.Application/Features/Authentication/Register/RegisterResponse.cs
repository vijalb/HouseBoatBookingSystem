namespace HouseBoat.Application.Features.Authentication.Register;

public sealed record RegisterResponse(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email);