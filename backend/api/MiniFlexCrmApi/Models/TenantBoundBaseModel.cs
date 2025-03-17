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

public record LinkedTenantBoundBaseModel : TenantBoundBaseModel
{
    public string? EntityName { get; set; } // Single entity to link to
    
    public int? EntityId { get; set; } // Single entity ID to link to
    
    public int? SignificanceOrdinal { get; set; } // Order the contact will appear in contact list
}