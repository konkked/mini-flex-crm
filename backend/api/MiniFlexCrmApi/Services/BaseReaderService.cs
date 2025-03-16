using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Services;

public abstract class BaseReaderService<TDbModel, TApiModel>(IRepo<TDbModel> repo) : IBaseReaderService<TApiModel>
    where TDbModel : DbEntity
{
    protected abstract TApiModel? DbModelToApiModel(TDbModel model);

    public virtual async Task<TApiModel?> GetAsync(int id)
    {
        var result = await repo.FindAsync(id).ConfigureAwait(false);
        return result is null 
            ? default 
            : DbModelToApiModel(result);
    }

    public Task<IEnumerable<TApiModel>> ListAsync() 
        => ListAsync(ServiceContants.PageSize);
    public Task<IEnumerable<TApiModel>> ListAsync(int limit) 
        => ListAsync(limit, 0);
    public Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset) 
        => ListAsync(limit, offset, null);

    public Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset, string? query)
        => ListAsync(limit, offset, query, default);

    public async Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset, string? query, 
        IDictionary<string, object>? parameters)
    {
        var returning = new List<TApiModel>();
        await foreach (var val in repo.GetSomeAsync(limit, offset, query, parameters).ConfigureAwait(false))
            returning.Add(DbModelToApiModel(val));
        return returning;
    }
}