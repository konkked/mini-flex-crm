using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Models;

public record UserModel : TenantBoundBaseModel
{
    public string Username { get; set; }
    public string? Role { get; set; }
    public bool Enabled { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? ProfileImage { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
}