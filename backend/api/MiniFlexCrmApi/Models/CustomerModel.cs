using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Models;

public record CustomerModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
    
    [JsonConverter(typeof(RelationshipsJsonConverter))]
    public Dictionary<string,dynamic?[]>? Relationships { get; set; }
}