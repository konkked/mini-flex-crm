using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepo userRepo, IJwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.FindByUsernameAsync(request.Username);

        if ((user?.Enabled ?? false) == false)
            return null;

        if (!VerifyPassword(request.Password, user.PasswordHash, user.Salt))
        {
            return null; // Return null for invalid login
        }

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) };
    }

    public async Task<AuthResponse?> SignUpAsync(SignUpRequest request)
    {
        if (await _userRepo.ExistsByUsernameAsync(request.Username))
        {
            return null; // Return null if username is taken
        }

        var salt = GenerateSalt();
        var hashedPassword = HashPassword(request.Password, salt);

        var newUser = new UserDbModel
        {
            Username = request.Username,
            PasswordHash = hashedPassword,
            Salt = salt,
            TenantId = request.TenantId
        };

        await _userRepo.CreateAsync(newUser).ConfigureAwait(false);
        return AuthResponseFromUser(newUser);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(TokenRefreshRequest expiredTokenRequest)
    {
        var expiredTokenData = _jwtService.ValidateThenGetTokenExpiry(expiredTokenRequest.Token);

        if (expiredTokenData == null
            || expiredTokenData.Value.Expiry >= TimeSpan.FromMinutes(JwtCustomConstants.RefreshWindowInMinutes)
            || expiredTokenData.Value.Expiry <= TimeSpan.FromMinutes(-JwtCustomConstants.RefreshWindowInMinutes))
            return null;

        var user = await _userRepo.FindAsync(expiredTokenData.Value.UserId)
            .ConfigureAwait(false);

        if (user == null)
            return null;

        return AuthResponseFromUser(user);
    }


    private AuthResponse? AuthResponseFromUser(UserDbModel user)
    {
        var token = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            Expiration = DateTime.UtcNow + JwtCustomConstants.TokenLifetime.Add(TimeSpan.FromMinutes(-1))
        };
    }

    private static string GenerateSalt()
    {
        var saltBytes = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string password, string salt)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password + salt)));

    private static bool VerifyPassword(string enteredPassword, string storedHash, string salt)
    {
        return HashPassword(enteredPassword, salt) == storedHash;
    }
}