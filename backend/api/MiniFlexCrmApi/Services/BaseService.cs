using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public abstract class BaseService<TDbModel,TApiModel>(IRepo<TDbModel> repo) 
        : BaseReaderService<TDbModel,TApiModel>(repo), IBaseService<TApiModel> where TDbModel : DbEntity
    where TApiModel: BaseApiModel
{
    protected abstract TDbModel ConvertToDbModel(TApiModel model);

    public virtual async Task<bool> DeleteItemAsync(int id) => await repo.DeleteAsync(id).ConfigureAwait(false) > 0;
    
    public async Task<bool> CreateItemAsync(TApiModel model) => await repo.CreateAsync(ConvertToDbModel(model)) > 0;

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

public interface ITenantBoundBaseService<TApiModel> : IBaseService<TApiModel> 
    where TApiModel : BaseApiModel
{
    Task<TApiModel> GetItemAsync(int tenantId, int id);
    Task<bool> DeleteItemAsync(int tenantId, int id);
}

public abstract class TenantBoundBaseService<TDbModel, TApiModel>(ITenantBoundDbEntityRepo<TDbModel> repo)
    : BaseService<TDbModel, TApiModel>(repo), ITenantBoundBaseService<TApiModel> where TDbModel : TenantBoundDbEntity
    where TApiModel : TenantBoundBaseModel
{
    public override async Task<bool> UpdateItemAsync(TApiModel model)
    {
        var dbModel = ConvertToDbModel(model);
        var current = await repo.FindInTenantById(model.TenantId, model.Id).ConfigureAwait(false);
        if (current == null) return false;
        Transpose.Pull(dbModel, current);
        return await repo.UpdateAsync(dbModel).ConfigureAwait(false) > 0;
    }

    public async Task<TApiModel> GetItemAsync(int tenantId, int id) 
        => ConvertToApiModel(await repo.FindInTenantById(tenantId, id));

    public Task<bool> DeleteItemAsync(int tenantId, int id) => repo.DeleteAsync(tenantId, id);
}