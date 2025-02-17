using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Auth;
using MiniFlexCrmApi.Api.Context;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Security;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
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
        if (response == null) return Forbid();

        return Ok(response);
    }
}