namespace MiniFlexCrmApi.Models.Public;

public class CustomerModel : TenantBoundBaseModel
{
    public string Name { get; set; }
    public dynamic Attributes { get; set; }
    public Dictionary<string,dynamic[]>? Relationships { get; set; }
}