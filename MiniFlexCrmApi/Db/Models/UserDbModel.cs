namespace MiniFlexCrmApi.Db.Models;

public class UserDbModel : TenantBoundDbEntity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    
    public bool Enabled { get; set; }
    
    public string Role { get; set; }
}