using InsurancePlatform.Api.Contracts;
using InsurancePlatform.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePlatform.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _authService.RegisterAsync(request, cancellationToken);
            return Created(string.Empty, new { message = "User registered successfully." });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { error = exception.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return Unauthorized(new { error = exception.Message });
        }
    }
}
