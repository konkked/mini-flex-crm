using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Reflect;

namespace MiniFlexCrmApi.Api.Services;

public abstract class BaseService<TDbModel,TApiModel> : BaseReaderService<TDbModel,TApiModel>, IBaseService<TApiModel> where TDbModel : DbEntity
    where TApiModel: BaseApiModel
{
    protected BaseService(IRepo<TDbModel> repo) : base(repo)
    {
    }
    
    protected abstract TDbModel ConvertToDbModel(TApiModel model);

    public virtual async Task<bool> DeleteItem(int id) => await Repo.DeleteAsync(id).ConfigureAwait(false) > 0;
    
    public async Task<bool> CreateItem(TApiModel model) => await Repo.CreateAsync(ConvertToDbModel(model)) > 0;

    public virtual async Task<bool> UpdateItem(TApiModel model)
    {
        var updating = ConvertToDbModel(model);
        var current = await Repo.FindAsync(model.Id).ConfigureAwait(false);
        if (current == null) 
            return false;
        Transpose.Pull(updating,current);
        return await Repo.UpdateAsync(updating).ConfigureAwait(false) > 0;
    }
}