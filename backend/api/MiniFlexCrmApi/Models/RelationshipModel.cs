using System.Text.Json.Serialization;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Serialization;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record RelationshipModel : BaseApiModel
{
    public int EntityId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EntityNameType EntityName { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
    
    public int AccountId { get; set; }
    public string? AccountName { get; set; }
    
    [IgnoreForUpdate]
    public int? TenantId { get; set; }
}

public enum EntityNameType
{
    account,
    company,
    user,
    lead,
    sale,
    sales_opportunity,
    interaction,
    note,
    unknown
}