namespace MiniFlexCrmApi.Db.Models;

public class SupportTicketDbModel : TenantBoundDbEntity
{
    public string Issue { get; set; }
    public string Status { get; set; }
    public dynamic? Attributes { get; set; }
    public int UserId { get; set; }
}