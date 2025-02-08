namespace MiniFlexCrmApi.Auth;

public class SignUpRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int TenantId { get; set; }
}