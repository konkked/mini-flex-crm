using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface ITenantRepo : IRepo<TenantDbModel>;

public class TenantRepo(IConnectionProvider connectionProvider) : DbEntityRepo<TenantDbModel>(connectionProvider), ITenantRepo
{
    public override Task<int> DeleteAsync(int id) => throw new NotImplementedException();
    public override Task<int> UpdateAsync(TenantDbModel model) => throw new NotImplementedException();
}