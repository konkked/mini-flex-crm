using System.Buffers.Text;

namespace MiniFlexCrmApi.Api.Services;

public class CursorTokenModel
{
    public int Id { get; set; }
    public int PageSize { get; set; }
}