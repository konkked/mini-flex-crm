using System.Buffers.Text;

namespace MiniFlexCrmApi.Api.Services;

public class NextTokenModel
{
    public int LastId { get; set; }
    public int PageSize { get; set; }
}