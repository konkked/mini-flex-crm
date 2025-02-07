namespace MiniFlexCrmApi.Models.Public;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    
    public string Tenant { get; set; }
}