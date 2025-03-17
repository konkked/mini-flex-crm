using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public abstract class BaseService<TDbModel,TApiModel>(IRepo<TDbModel> repo) 
        : BaseReaderService<TDbModel,TApiModel>(repo), IBaseService<TApiModel> where TDbModel : DbEntity
    where TApiModel: BaseApiModel
{
    protected abstract TDbModel ApiModelToDbModel(TApiModel model);

    public virtual async Task<bool> DeleteAsync(int id) => await repo.DeleteAsync(id).ConfigureAwait(false) > 0;
    
    public Task<int> CreateAsync(TApiModel model) => repo.CreateAsync(ApiModelToDbModel(model));

    public virtual async Task<bool> UpdateAsync(TApiModel model)
    {
        var updating = ApiModelToDbModel(model);
        var current = await repo.FindAsync(model.Id).ConfigureAwait(false);
        if (current == null) 
            return false;
        Transpose.Pull(updating,current);
        return await repo.UpdateAsync(updating).ConfigureAwait(false) > 0;
    }
    
    public virtual async Task<int> UpsertAsync(TApiModel model)
    {
        var current = await repo.FindAsync(model.Id).ConfigureAwait(false);
        if (current == null)
        {
            var result = await CreateAsync(model).ConfigureAwait(false);
            return result;
        }
        var dbModel = ApiModelToDbModel(model);
        Transpose.Pull(dbModel, current);
        if (await repo.UpdateAsync(dbModel).ConfigureAwait(false) > 0)
            return dbModel.Id;
        return -1;
    }
}