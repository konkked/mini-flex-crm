namespace MiniFlexCrmApi.Db.Models;

public class DbEntity
{
    public int Id { get; set; }
    
    [IgnoreForUpdate]
    public int? UpdatedTs { get; set; }
}