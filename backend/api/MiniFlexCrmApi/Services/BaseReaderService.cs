using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public abstract class BaseReaderService<TDbModel, TApiModel>(IRepo<TDbModel> repo) : IBaseReaderService<TApiModel>
    where TDbModel : DbEntity
{
    protected abstract TApiModel ConvertToApiModel(TDbModel model);
    
    public virtual async Task<TApiModel> GetItem(int id) => 
        ConvertToApiModel(await repo.FindAsync(id).ConfigureAwait(false));
    
    public Task<Page<TApiModel>> ListItems() => ListItems(ServiceContants.PageSize);
    public Task<Page<TApiModel>> ListItems(int pageSize) => ListItems(pageSize, null);
    public Task<Page<TApiModel>> ListItems(int pageSize, string? next) => ListItems(pageSize, next, null);
    public async Task<Page<TApiModel>> ListItems(int pageSize, string? next, string? query)
    {
        var prevToken = string.IsNullOrEmpty(next) 
            ? new CursorTokenModel {Id = 0, PageSize = pageSize } 
            : Base62JsonConverter.Deserialize<CursorTokenModel>(next);
        pageSize = prevToken.PageSize;
        var returning = new List<TApiModel>();
        var id = 0;
        await foreach (var val in repo.GetNext(prevToken.Id,prevToken.PageSize, query).ConfigureAwait(false))
        {
            id = val.Id;
            returning.Add(ConvertToApiModel(val));
        }

        return new()
        {
            Items = returning,
            Next = returning.Count >= pageSize
                ? Base62JsonConverter.Serialize(new CursorTokenModel { Id = id, PageSize = pageSize })
                : null,
            Prev = next
        };
    }

    public Task<Page<TApiModel>> ListPreviousItems(int pageSize, string? prev) =>  ListPreviousItems(pageSize, prev, null);
    
    public async Task<Page<TApiModel>> ListPreviousItems(int pageSize, string? prev, string? query) 
    {
        var prevToken = string.IsNullOrEmpty(prev) 
            ? new CursorTokenModel {Id = 0, PageSize = pageSize } 
            : Base62JsonConverter.Deserialize<CursorTokenModel>(prev);
        pageSize = prevToken.PageSize;
        var returning = new Stack<TApiModel>();
        var id = 0;
        await foreach (var val in repo.GetPrevious(prevToken.Id,prevToken.PageSize + 1, query).ConfigureAwait(false))
        {
            returning.Push(ConvertToApiModel(val));
            pageSize--;
            if (pageSize == 0)
                id = val.Id;
        }
        
        return new()
            {
                Items = returning,
                Prev = id == 0 
                    ? null 
                    : Base62JsonConverter.Serialize(new CursorTokenModel { Id =  id, PageSize = pageSize }),
                Next = prev 
            };
    }
}