using System.Text.Json.Serialization;
using MiniFlexCrmApi.Serialization;

namespace MiniFlexCrmApi.Models;

public record AccountModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
    
    public UserModel AccountManager { get; set; }
    public UserModel SalesRep { get; set; }
    public UserModel PreSalesRep { get; set; }
    
    [JsonConverter(typeof(RelationshipsJsonConverter))]
    public Dictionary<string,dynamic?[]>? Relationships { get; set; }
}

public record TeamMemberModel
{
    public string Role { get; set; }
    public UserModel User { get; set; }
}

public record BaseTeamModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
    public UserModel Owner { get; set; }
}

public record TeamModel : BaseTeamModel
{
    public TeamMemberModel[] Members { get; set; }
    public AccountModel[] Accounts { get; set; }
}