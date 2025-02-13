namespace MiniFlexCrmApi.Api.Services;

public class Page<T>
{
    public IEnumerable<T> Items { get; set; }
    public string? Next { get; set; }
    public string? Prev { get; set; }
}