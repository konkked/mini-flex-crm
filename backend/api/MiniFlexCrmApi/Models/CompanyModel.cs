namespace MiniFlexCrmApi.Api.Models;

public record CompanyModel : TenantBoundBaseModel
{
    public string? Name { get; set; }
}