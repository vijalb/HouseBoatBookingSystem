namespace HouseBoat.Application.Features.Authentication.Login;

public sealed record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc);