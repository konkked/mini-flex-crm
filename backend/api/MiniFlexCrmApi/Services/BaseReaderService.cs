using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Services;

public abstract class BaseReaderService<TDbModel, TApiModel>(IRepo<TDbModel> repo) : IBaseReaderService<TApiModel>
    where TDbModel : DbEntity
{
    protected abstract TApiModel ConvertToApiModel(TDbModel model);
    
    public virtual async Task<TApiModel> GetItemAsync(int id) => 
        ConvertToApiModel(await repo.FindAsync(id).ConfigureAwait(false));
    public Task<IEnumerable<TApiModel>> ListItemsAsync() => ListItemsAsync(ServiceContants.PageSize);
    public Task<IEnumerable<TApiModel>> ListItemsAsync(int limit) => ListItemsAsync(limit, 0);
    public Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset) => ListItemsAsync(limit, offset, null);

    public Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset, string? query)
        => ListItemsAsync(limit, offset, query, default);

    public async Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset, string? query, IDictionary<string, object>? parameters)
    {
        var returning = new List<TApiModel>();
        await foreach (var val in repo.GetSomeAsync(limit, offset, query, parameters).ConfigureAwait(false))
            returning.Add(ConvertToApiModel(val));
        return returning;
    }
}