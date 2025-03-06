using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Auth;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Logs in a user and returns a JWT token if credentials are valid.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null) return Unauthorized(new { message = "Invalid username or password" });

        return Ok(response);
    }

    /// <summary>
    /// Registers a new user and returns a JWT token.
    /// </summary>
    [HttpPost("signup")]
    public async Task<ActionResult<AuthResponse>> SignUp(
        [FromBody] SignUpRequest request)
    {
        var response = await _authService.SignUpAsync(request);
        if (response == null) return BadRequest(new { message = "Username already taken" });

        return Ok(response);
    }

    /// <summary>
    /// Refreshes an expired JWT token.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        [FromBody] TokenRefreshRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        if (response == null) return Forbid();

        return Ok(response);
    }

    [HttpPost("verify_email")]
    public async Task<ActionResult<bool>> VerifyEmail([FromQuery] string token)
    {
        return Ok(await _authService.VerifyEmailAsync(token));
    }
}