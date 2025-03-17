using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public abstract class TenantBoundBaseService<TDbModel, TApiModel>(ITenantBoundDbEntityRepo<TDbModel> repo)
    : BaseService<TDbModel, TApiModel>(repo), ITenantBoundBaseService<TApiModel> where TDbModel : TenantBoundDbEntity
    where TApiModel : TenantBoundBaseModel
{
    public override async Task<int> UpsertAsync(TApiModel model)
    {
        var current = await repo.FindInTenantById(model.TenantId, model.Id).ConfigureAwait(false);
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

    public override async Task<bool> UpdateAsync(TApiModel model)
    {
        var dbModel = ApiModelToDbModel(model);
        var current = await repo.FindInTenantById(model.TenantId, model.Id).ConfigureAwait(false);
        if (current == null) return false;
        Transpose.Pull(dbModel, current);
        return await repo.UpdateAsync(dbModel).ConfigureAwait(false) > 0;
    }

    public async Task<TApiModel> GetAsync(int tenantId, int id) 
        => DbModelToApiModel(await repo.FindInTenantById(tenantId, id).ConfigureAwait(false));

    public Task<bool> DeleteAsync(int tenantId, int id) 
        => repo.DeleteAsync(tenantId, id);
}