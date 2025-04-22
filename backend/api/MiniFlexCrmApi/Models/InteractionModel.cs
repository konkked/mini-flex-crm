using System.Text.Json.Serialization;

namespace MiniFlexCrmApi.Models;

public record InteractionModel : TenantBoundBaseModel
{
    public int Id { get; set; }
    
    public InteractionTarget Target { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InteractionType Type { get; set; } // Changed from string to InteractionType
    public DateTime InteractionDate { get; set; }
    
    public string? Notes { get; set; }
    public dynamic? Attributes { get; set; }
}

public record InteractionTarget
{
    public int? AccountId { get; set; }
    public int? ContactId { get; set; }
    public int? LeadId { get; set; }
    public int? DealId { get; set; }
}