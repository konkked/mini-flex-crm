using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

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
        => ConvertToApiModel(await repo.FindInTenantById(tenantId, id).ConfigureAwait(false));

    public Task<bool> DeleteItemAsync(int tenantId, int id) 
        => repo.DeleteAsync(tenantId, id);
}