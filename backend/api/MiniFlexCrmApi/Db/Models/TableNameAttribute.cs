namespace MiniFlexCrmApi.Db.Models;

public class TableNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}