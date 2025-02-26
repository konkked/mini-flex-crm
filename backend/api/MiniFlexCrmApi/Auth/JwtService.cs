using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Api.Auth;

public interface IJwtService
{
    string GenerateToken(UserDbModel user);

    /// <summary>
    /// Validates a JWT token and returns:
    /// - (UserId, TimeSpan until expiration) if valid
    /// - null if expired or signature does not match
    /// </summary>
    (int UserId, TimeSpan Expiry)? ValidateThenGetTokenExpiry(string token);
}

public class JwtService : IJwtService
{
    private readonly string _jwtSecret;

    public JwtService(IEncryptionSecretProvider secretProvider)
    {
        _jwtSecret = secretProvider.GetSecret();
    }

    public string GenerateToken(UserDbModel user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new (JwtRegisteredClaimNames.UniqueName, user.Username),
                new (ServerCustomConstants.ClaimTypes.TenantId, user.TenantId.ToString()),
                new ( ServerCustomConstants.ClaimTypes.Role, user.Role),
            }),
            Expires = DateTime.UtcNow + ServerCustomConstants.TokenLifetime,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    /// <summary>
    /// Validates a JWT token and returns:
    /// - (UserId, TimeSpan until expiration) if valid
    /// - null if expired or signature does not match
    /// </summary>
    public (int UserId, TimeSpan Expiry)? ValidateThenGetTokenExpiry(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken)
            {
                var expiration = jwtToken.ValidTo;
                var userId = int.TryParse(
                    principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userIdInt) 
                    ? userIdInt : 0;

                return (userId, expiration - DateTime.UtcNow);
            }
        }
        catch (SecurityTokenException)
        {
            return null; // Invalid signature
        }

        return null; // Any other failure
    }
}