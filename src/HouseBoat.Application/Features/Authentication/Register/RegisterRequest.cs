namespace HouseBoat.Application.Features.Authentication.Register;

public sealed record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber);