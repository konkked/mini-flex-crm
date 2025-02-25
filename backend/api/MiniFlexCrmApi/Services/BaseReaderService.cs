using System.Collections.Generic;
using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public abstract class BaseReaderService<TDbModel, TApiModel>(IRepo<TDbModel> repo) : IBaseReaderService<TApiModel>
    where TDbModel : DbEntity
{
    protected abstract TApiModel ConvertToApiModel(TDbModel model);
    
    public virtual async Task<TApiModel> GetItem(int id) => 
        ConvertToApiModel(await repo.FindAsync(id).ConfigureAwait(false));
    public Task<IEnumerable<TApiModel>> ListItems() => ListItems(ServiceContants.PageSize);
    public Task<IEnumerable<TApiModel>> ListItems(int limit) => ListItems(limit, 0);
    public Task<IEnumerable<TApiModel>> ListItems(int limit, int offset) => ListItems(limit, offset, null);
    public async Task<IEnumerable<TApiModel>> ListItems(int limit, int offset, string? query)
    {
        var returning = new List<TApiModel>();
        await foreach (var val in repo.GetSomeAsync(limit, offset, query).ConfigureAwait(false))
            returning.Add(ConvertToApiModel(val));
        return returning;
    }
}