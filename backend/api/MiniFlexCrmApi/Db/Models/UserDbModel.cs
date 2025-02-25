using System.Text.Json.Serialization;
using MiniFlexCrmApi.Api.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class UserDbModel : TenantBoundDbEntity
{
    public string? Username { get; set; }
    
    public string Email { get; set; }
    
    public string Name { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }
    
    public bool? Enabled { get; set; }
    
    public string? Role { get; set; }
}