using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Auth;

public class JwtAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserRepo _userRepo;
    private readonly string _jwtSecret;

    public JwtAuthorizationMiddleware(RequestDelegate next, IEncryptionSecretProvider encryptionSecretProvider, IUserRepo userRepo)
    {
        _next = next;
        _userRepo = userRepo;
        _jwtSecret = encryptionSecretProvider.GetSecret();
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid token.");
            return;
        }
        
        var requestPath = context.Request.Path.Value?.Trim('/').Split('/');
        var pathTenantId = ExtractTenantIdFromPath(requestPath);

        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);
        try
        {
            var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            int? tokenTenantId = null;
            if (int.TryParse(
                    claimsPrincipal.FindFirstValue(ServerCustomConstants.ClaimTypes.TenantId),
                    out var tid))
            {
                tokenTenantId = tid;
            }

            if (!tokenTenantId.HasValue)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }
            
            // only super user can access tenant apis
            if (!pathTenantId.HasValue 
                && context.Request.Path.Value?.StartsWith("/api/tenant/") == true 
                && tokenTenantId.Value != 0)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden.");
                return;
            }
            
            // if the path tenant id does not match the token tenant id
            // and the user is not a super user (tenant id of 0) 
            // then request is unauthorized.
            if (pathTenantId != tokenTenantId && tokenTenantId != 0)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized.");
                return;
            }

            if (int.TryParse(claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var id)
                && await _userRepo.IsEnabledAsync(id).ConfigureAwait(false))
            {
                // Store validated claims in HttpContext.User
                context.User = claimsPrincipal;
                await _next(context);
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token.");
        }
        catch (SecurityTokenExpiredException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token expired.");
        }
        catch (SecurityTokenException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid token.");
        }
    }
    

    /// <summary>
    /// Extracts the Tenant ID from the request path dynamically.
    /// </summary>
    private static int? ExtractTenantIdFromPath(string[] requestPath)
    {
        if (requestPath.Length < 2) return null;

        for (var i = 0; i < requestPath.Length - 1; i++)
        {
            if (requestPath[i].Equals("tenant", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(requestPath[i + 1], out var tenantId))
            {
                return tenantId;
            }
        }
        return null;
    }
    
}