namespace MiniFlexCrmApi.Models;

public record ContactModel : LinkedTenantBoundBaseModel
{
    public string Name { get; set; }
    public string? Title { get; set; }
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public string? Phone { get; set; }
    public bool? PhoneVerified { get; set; }
    public bool CanText { get; set; }
    public bool CanCall { get; set; }
    public bool CanEmail { get; set; }
    public dynamic? Attributes { get; set; }
}