using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Middleware;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Auth;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> Login(
        [FromRequestContext] RequestContext requestContext,
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
    public async Task<IActionResult> SignUp(
        [FromRequestContext] RequestContext requestContext,
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
    public async Task<IActionResult> Refresh(
        [FromRequestContext] RequestContext requestContext,
        [FromBody] TokenRefreshRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);
        if (response == null) return Unauthorized(new { message = "Invalid or expired refresh token" });

        return Ok(response);
    }
    
    
}