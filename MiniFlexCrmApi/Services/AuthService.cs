using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Models;


public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}


public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(AuthRequest request);
    Task<AuthResponse?> SignUpAsync(AuthRequest request);
    Task<AuthResponse?> RefreshTokenAsync(AuthResponse expiredToken);
}