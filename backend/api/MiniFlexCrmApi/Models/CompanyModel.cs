using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Models;

public record CompanyModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
}