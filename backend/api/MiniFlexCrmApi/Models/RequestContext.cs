namespace MiniFlexCrmApi.Models;

public class RequestContext
{
    public string? Method { get; set; }
    public string? Path { get; set; }
    public string? QueryString { get; set; }
    public string? ContentType { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, string>? Cookies { get; set; }
    public int? TenantId { get; set; }
    public string? Role { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public Dictionary<string, string>? Claims { get; set; }
}