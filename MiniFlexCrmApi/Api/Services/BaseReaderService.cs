using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public abstract class BaseReaderService<TDbModel,TApiModel> : IBaseReaderService<TApiModel> where TDbModel : DbEntity
{
    protected readonly IRepo<TDbModel> Repo;

    protected BaseReaderService(IRepo<TDbModel> repo)
    {
        Repo = repo;
    }
    
    protected abstract TApiModel ConvertToApiModel(TDbModel model);
    
    public virtual async Task<TApiModel> GetItem(int id) => 
        ConvertToApiModel(await Repo.FindAsync(id).ConfigureAwait(false));
    
    public Task<Page<TApiModel>> ListItems() => ListItems(ServiceContants.PageSize);
    public Task<Page<TApiModel>> ListItems(int pageSize) => ListItems(pageSize, null);
    public async Task<Page<TApiModel>> ListItems(int pageSize, string? next)
    {
        var prevToken = string.IsNullOrEmpty(next) 
            ? new NextTokenModel {LastId = 0, PageSize = pageSize } 
            : Base62JsonConverter.Deserialize<NextTokenModel>(next);
        var returning = new List<TApiModel>();
        var lastId = 0;
        await foreach (var user in Repo.GetNext(prevToken.LastId,prevToken.PageSize).ConfigureAwait(false))
        {
            lastId = user.Id;
            returning.Add(ConvertToApiModel(user));
        }

        return new()
        {
            Items = returning,
            Next = returning.Count >= pageSize
                ? Base62JsonConverter.Serialize(new NextTokenModel { LastId = lastId, PageSize = pageSize })
                : null
        };
    }
}