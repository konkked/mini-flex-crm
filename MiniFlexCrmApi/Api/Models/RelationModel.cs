using System.Text.Json.Serialization;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Api.Models;

public record RelationModel : BaseApiModel
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