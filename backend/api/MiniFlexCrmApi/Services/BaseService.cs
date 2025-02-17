using System.Threading.Tasks;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Api.Services;

public abstract class BaseService<TDbModel,TApiModel>(IRepo<TDbModel> repo) 
        : BaseReaderService<TDbModel,TApiModel>(repo), IBaseService<TApiModel> where TDbModel : DbEntity
    where TApiModel: BaseApiModel
{
    protected abstract TDbModel ConvertToDbModel(TApiModel model);

    public virtual async Task<bool> DeleteItem(int id) => await repo.DeleteAsync(id).ConfigureAwait(false) > 0;
    
    public async Task<bool> CreateItem(TApiModel model) => await repo.CreateAsync(ConvertToDbModel(model)) > 0;

    public virtual async Task<bool> UpdateItem(TApiModel model)
    {
        var updating = ConvertToDbModel(model);
        var current = await repo.FindAsync(model.Id).ConfigureAwait(false);
        if (current == null) 
            return false;
        Transpose.Pull(updating,current);
        return await repo.UpdateAsync(updating).ConfigureAwait(false) > 0;
    }
}