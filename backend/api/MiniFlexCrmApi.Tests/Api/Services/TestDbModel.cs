using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Tests.Api.Services;

public class TestDbModel : DbEntity
{
    public string Name { get; set; }
}