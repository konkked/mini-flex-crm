using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Models;

public record TenantModel : BaseApiModel
{
    public required string Name { get; set; }
}