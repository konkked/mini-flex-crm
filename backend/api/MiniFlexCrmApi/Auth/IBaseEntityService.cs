using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Auth;

public interface IBaseEntityService<T>
{
    Task<T?> FindAsync(RequestContext context, int id);
    Task<T?> CreateAsync(RequestContext context, string id);
    Task<bool> DeleteAsync(RequestContext context, int id);
    IAsyncEnumerable<T> ListAsync(RequestContext context, out string nextToken);
}