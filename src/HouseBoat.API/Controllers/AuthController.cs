using FluentValidation;
using HouseBoat.Application.Features.Authentication.Login;
using HouseBoat.Application.Features.Authentication.Register;
using Microsoft.AspNetCore.Mvc;

namespace HouseBoat.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegisterHandler _registerHandler;
    private readonly IValidator<RegisterRequest> _registerValidator;

    private readonly LoginHandler _loginHandler;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(
        RegisterHandler registerHandler,
        IValidator<RegisterRequest> registerValidator,
        LoginHandler loginHandler,
        IValidator<LoginRequest> loginValidator)
    {
        _registerHandler = registerHandler;
        _registerValidator = registerValidator;
        _loginHandler = loginHandler;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _registerValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _registerHandler.HandleAsync(
            request,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult =
            await _loginValidator.ValidateAsync(
                request,
                cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var response = await _loginHandler.HandleAsync(
            request,
            cancellationToken);

        return Ok(response);
    }
}