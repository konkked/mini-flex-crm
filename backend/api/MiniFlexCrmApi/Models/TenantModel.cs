using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Models;

public record TenantModel : BaseApiModel
{
    public required string Name { get; set; }
}