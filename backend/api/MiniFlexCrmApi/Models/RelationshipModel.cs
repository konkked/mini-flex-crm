using System.Text.Json.Serialization;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record RelationshipModel : BaseApiModel
{
    public int EntityId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EntityNameType EntityName { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    
    [IgnoreForUpdate]
    public int? TenantId { get; set; }
}

public enum EntityNameType
{
    customer,
    company,
    user
}