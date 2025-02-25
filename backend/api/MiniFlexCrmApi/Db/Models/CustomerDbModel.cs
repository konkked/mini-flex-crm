using System.Text.Json.Serialization;
using MiniFlexCrmApi.Api.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class CustomerDbModel: TenantBoundDbEntity
{
    public string? Name { get; set; }
    
    // Map JSON attributes as a dictionary or a specific type
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
}