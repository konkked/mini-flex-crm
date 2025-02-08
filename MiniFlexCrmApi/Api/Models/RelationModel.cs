using System.Text.Json.Serialization;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Models;

public record RelationModel : BaseApiModel
{
    public int EntityId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EntityNameType EntityName { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

public enum EntityNameType
{
    customer,
    company,
    user
}