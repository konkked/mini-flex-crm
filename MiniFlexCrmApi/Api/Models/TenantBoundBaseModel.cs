using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Models.Public;

public class TenantBoundBaseModel : BaseApiModel
{
    public string Tenant { get; set; }
    public int TenantId { get; set; }
}