using System.Collections.Generic;
using System.Text.Json.Serialization;
using MiniFlexCrmApi.Api.Serialization;

namespace MiniFlexCrmApi.Api.Models;

public record CustomerModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
    
    [JsonConverter(typeof(RelationshipsJsonConverter))]
    public Dictionary<string,dynamic?[]>? Relationships { get; set; }
}