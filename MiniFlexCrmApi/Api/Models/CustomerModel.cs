namespace MiniFlexCrmApi.Api.Models;

public class CustomerModel : TenantBoundBaseModel
{
    public string Name { get; set; }
    public dynamic Attributes { get; set; }
    public Dictionary<string,dynamic[]>? Relationships { get; set; }
}