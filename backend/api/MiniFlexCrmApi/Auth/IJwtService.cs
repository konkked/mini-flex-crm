using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Auth;

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