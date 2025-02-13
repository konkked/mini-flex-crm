namespace MiniFlexCrmApi.Api.Auth;

public class SignUpRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required int TenantId { get; set; }
}