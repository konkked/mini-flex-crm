using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Db.Models;

[TableName("app_user")]
public class UserDbModel : TenantBoundDbEntity
{
    public string Username { get; set; }
    
    public string Email { get; set; }
    
    public string Name { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    
    public bool Enabled { get; set; }
    
    public string Role { get; set; }
    
    public string? ProfileImage { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
}