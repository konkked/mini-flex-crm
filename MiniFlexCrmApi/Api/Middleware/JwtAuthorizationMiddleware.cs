
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniFlexCrmApi.Api.Models;


public class JwtAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtSecret;

    public JwtAuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _jwtSecret = configuration["MINIFLEXCRMAPI_JWT_KEY"] 
                     ?? throw new ArgumentNullException("JWT secret key is missing.");
    }

    public async Task Invoke(HttpContext context)
    {
        var requestPath = context.Request.Path.Value?.Trim('/').Split('/');
        int? tenantId = ExtractTenantIdFromPath(requestPath);

        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing or invalid token.");
            return;
        }

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

            var userTenantId = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
            var userRole = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // If the request is for a specific tenant, ensure the user has access
            if (tenantId.HasValue && userTenantId != tenantId.Value.ToString())
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("You do not have access to this tenant.");
                return;
            }

            // Populate RequestContext with the extracted tenant and role
            var requestContext = new RequestContext
            {
                TenantId = userTenantId != null ? int.Parse(userTenantId) : null,
                Role = userRole
            };
            context.Items["RequestContext"] = requestContext;

            await _next(context);
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

        for (int i = 0; i < requestPath.Length - 1; i++)
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