using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record TenantModel : BaseApiModel
{
    public required string Name { get; set; }
    
    public string Theme { get; set; }
    
    public string ShortId { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
    
    public string Base64Logo { get; set; }
    
    public string Base64Banner { get; set; }
}