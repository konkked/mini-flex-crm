using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Models;

public record TenantBoundBaseModel : BaseApiModel
{
    public string Tenant { get; set; }
    public int TenantId { get; set; }
}