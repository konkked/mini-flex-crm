using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Db.Models;

public class AccountDbModel: TenantBoundDbEntity
{
    public string? Name { get; set; }
    
    public int? AccountManagerId { get; set; }
    public UserDbModel AccountManager { get; set; }
    
    public int? SalesRepId { get; set; }
    public UserDbModel SalesRep { get; set; }
    
    public int? PreSalesRepId { get; set; }
    public UserDbModel PreSalesRep { get; set; }
    
    // Map JSON attributes as a dictionary or a specific type
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
}


public class TeamDbModel: TenantBoundDbEntity
{
    public UserDbModel? Owner { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TeamMemberDbModel[] Members { get; set; }
    
    public AccountDbModel[] Accounts { get; set; }
    
    // Map JSON attributes as a dictionary or a specific type
    [JsonConverter(typeof(AttributesJsonConverter))]
    public dynamic? Attributes { get; set; }
}

public class TeamMemberDbModel
{
    public string Role { get; set; }
    public UserDbModel User { get; set; }
}