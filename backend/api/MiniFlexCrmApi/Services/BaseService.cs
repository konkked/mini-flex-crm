using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public abstract class BaseService<TDbModel,TApiModel>(IRepo<TDbModel> repo) 
        : BaseReaderService<TDbModel,TApiModel>(repo), IBaseService<TApiModel> where TDbModel : DbEntity
    where TApiModel: BaseApiModel
{
    protected abstract TDbModel ConvertToDbModel(TApiModel model);

    public virtual async Task<bool> DeleteItemAsync(int id) => await repo.DeleteAsync(id).ConfigureAwait(false) > 0;
    
    public Task<int> CreateItemAsync(TApiModel model) => repo.CreateAsync(ConvertToDbModel(model));

    public virtual async Task<bool> UpdateItemAsync(TApiModel model)
    {
        var updating = ConvertToDbModel(model);
        var current = await repo.FindAsync(model.Id).ConfigureAwait(false);
        if (current == null) 
            return false;
        Transpose.Pull(updating,current);
        return await repo.UpdateAsync(updating).ConfigureAwait(false) > 0;
    }
}