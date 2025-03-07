using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class CompanyDbModel : TenantBoundDbEntity
{
    public string? Name { get; set; }
    
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic Attributes { get; set; }
}