namespace MiniFlexCrmApi.Api.Models;

public class UserModel : TenantBoundBaseModel
{
    
    public string Username { get; set; }
    
    public string Role { get; set; }
    public bool Enabled { get; set; }
}