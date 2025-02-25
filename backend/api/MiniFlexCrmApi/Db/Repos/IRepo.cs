using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniFlexCrmApi.Db.Repos;

public interface IRepo<T> where T : class
{
    Task<bool> ExistsAsync(int id);
    IAsyncEnumerable<T> GetSomeAsync();
    IAsyncEnumerable<T> GetSomeAsync(int limit);
    IAsyncEnumerable<T> GetSomeAsync(int limit, int offset);
    IAsyncEnumerable<T> GetSomeAsync(int limit, int offset, string query);
    Task<T?> FindAsync(int id);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
    Task<int> CreateAsync(T entity);
}