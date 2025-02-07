using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Auth;

public class SignUpRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int TenantId { get; set; }
}

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
    Task<AuthResponse?> SignUpAsync(SignUpRequest request);
    Task<AuthResponse?> RefreshTokenAsync(AuthResponse expiredToken);
}


public class JwtService
{
    private readonly string _jwtSecret;
    private readonly int _jwtExpirationMinutes = 60;

    public JwtService(IConfiguration configuration)
    {
        _jwtSecret = configuration["MINIFLEXCRMAPI_JWT_KEY"] 
                     ?? throw new ArgumentNullException("JWT secret key is missing.");
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
                new ("tenant_id", user.TenantId.ToString()),
                new ("role", user.Role),
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    
    /// <summary>
    /// Validates a JWT token.
    /// Returns:
    /// - true if valid
    /// - false if signature does not match
    /// - null if expired
    /// </summary>
    public bool? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true, // Ensures expiration check is performed
                ClockSkew = TimeSpan.Zero // No leeway for expired tokens
            }, out SecurityToken validatedToken);

            return true; // Token is valid
        }
        catch (SecurityTokenExpiredException)
        {
            return null; // Token is expired
        }
        catch (SecurityTokenException)
        {
            return false; // Token signature is invalid
        }
    }
}

public class AuthService : IAuthService
{
    private readonly IUserRepo _userRepo;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepo userRepo, JwtService jwtService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse?> LoginAsync(AuthRequest request)
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

        await _userRepo.CreateAsync(newUser);
        var token = _jwtService.GenerateToken(newUser);

        return new AuthResponse { Token = token, Expiration = DateTime.UtcNow.AddMinutes(60) };
    }

    public Task<AuthResponse?> RefreshTokenAsync(AuthResponse expiredToken)
    {
        // Future implementation: Validate and refresh the JWT
        return Task.FromResult<AuthResponse?>(null);
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