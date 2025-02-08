namespace MiniFlexCrmApi.Api.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> SignUpAsync(SignUpRequest request);
    Task<AuthResponse?> RefreshTokenAsync(TokenRefreshRequest expiredToken);
}