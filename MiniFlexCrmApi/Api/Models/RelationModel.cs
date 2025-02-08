using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Models.Public;

public class RelationModel : BaseApiModel
{
    public int EntityId { get; set; }
    public string EntityName { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
}