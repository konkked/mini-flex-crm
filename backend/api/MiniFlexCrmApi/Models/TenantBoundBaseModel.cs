using System.Text.Json.Serialization;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Serialization;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record TenantBoundBaseModel : BaseApiModel
{
    public string Tenant { get; set; }
    public int TenantId { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
    
}