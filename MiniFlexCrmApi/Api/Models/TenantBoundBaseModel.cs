using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Models;

public class TenantBoundBaseModel : BaseApiModel
{
    public string Tenant { get; set; }
    public int TenantId { get; set; }
}