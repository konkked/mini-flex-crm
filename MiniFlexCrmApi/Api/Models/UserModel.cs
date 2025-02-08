using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Models.Public;

public class UserModel : TenantBoundBaseModel
{
    
    public string Username { get; set; }
    
    public string Role { get; set; }
    public bool Enabled { get; set; }
}