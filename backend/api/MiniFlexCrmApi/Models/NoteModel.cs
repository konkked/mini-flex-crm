namespace MiniFlexCrmApi.Models;


public class NoteModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
    public string Route { get; set; }
    
    public bool Pinned { get; set; }
}