namespace MiniFlexCrmApi.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<int> SignUpAsync(SignUpRequest request);
    Task<AuthResponse?> RefreshTokenAsync(TokenRefreshRequest expiredToken);
    Task<bool> VerifyEmailAsync(string token);
}