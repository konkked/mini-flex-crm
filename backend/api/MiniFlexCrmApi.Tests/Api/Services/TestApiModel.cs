using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Tests.Api.Services;

public record TestApiModel : BaseApiModel
{
    public string Name { get; set; }
}