namespace MiniFlexCrmApi.Models;

public record SupportTicketModel : TenantBoundBaseModel
{
    public string Issue { get; set; }
    public SupportTicketStatusType Status { get; set; }
    public dynamic? Attributes { get; set; }
    public int UserId { get; set; }
}