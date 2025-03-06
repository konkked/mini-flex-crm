namespace MiniFlexCrmApi.Auth;

public class AuthRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}