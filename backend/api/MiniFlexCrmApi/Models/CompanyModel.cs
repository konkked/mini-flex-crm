namespace MiniFlexCrmApi.Models;

public record CompanyModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
}