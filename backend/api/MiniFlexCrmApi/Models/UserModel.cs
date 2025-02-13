namespace MiniFlexCrmApi.Api.Models;

public record UserModel : TenantBoundBaseModel
{
    public string? Username { get; set; }
    public string? Role { get; set; }
    public bool? Enabled { get; set; }
}