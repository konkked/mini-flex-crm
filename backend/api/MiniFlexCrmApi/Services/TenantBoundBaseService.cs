using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Services;

public abstract class TenantBoundBaseService<TDbModel, TApiModel> : BaseService<TDbModel, TApiModel>
    where TDbModel : TenantBoundDbEntity
    where TApiModel : TenantBoundBaseModel
{
    private readonly ITenantBoundDbEntityRepo<TDbModel> _repo;

    protected TenantBoundBaseService(ITenantBoundDbEntityRepo<TDbModel> repo) : base(repo)
    {
        _repo = repo;
    }

    public virtual async Task<TApiModel?> GetAsync(int tenantId, int id)
    {
        var model = await _repo.FindBound(id).ConfigureAwait(false);
        return model == null ? null : DbModelToApiModel(model);
    }

    public virtual async Task<bool> UpdateAsync(TApiModel model)
    {
        var dbModel = ApiModelToDbModel(model);
        var current = await _repo.FindBound(model.Id).ConfigureAwait(false);
        if (current == null) return false;
        
        return await _repo.UpdateAsync(dbModel).ConfigureAwait(false) > 0;
    }

    public virtual async Task<bool> DeleteAsync(int tenantId, int id)
    {
        return await _repo.DeleteAsync(tenantId, id);
    }

    public virtual async Task<int> CreateAsync(TApiModel model)
    {
        var dbModel = ApiModelToDbModel(model);
        return await _repo.CreateAsync(dbModel).ConfigureAwait(false);
    }

    public virtual async Task<int> UpsertAsync(TApiModel model)
    {
        var current = await _repo.FindBound(model.Id).ConfigureAwait(false);
        if (current == null)
        {
            return await CreateAsync(model).ConfigureAwait(false);
        }

        var dbModel = ApiModelToDbModel(model);
        if (await _repo.UpdateAsync(dbModel).ConfigureAwait(false) > 0)
            return dbModel.Id;
        return -1;
    }

    protected abstract override TApiModel DbModelToApiModel(TDbModel model);
    protected abstract override TDbModel ApiModelToDbModel(TApiModel model);
}