using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record TenantBoundBaseModel : BaseApiModel
{
    public string Tenant { get; set; }
    public int TenantId { get; set; }
}