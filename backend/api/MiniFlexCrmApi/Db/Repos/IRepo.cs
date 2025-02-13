namespace MiniFlexCrmApi.Db.Repos;

public interface IRepo<T> where T : class
{
    Task<bool> ExistsAsync(int id);
    IAsyncEnumerable<T> GetSome(int count);
    IAsyncEnumerable<T> GetNext(int id, int count);
    IAsyncEnumerable<T> GetPrevious(int id, int count);
    IAsyncEnumerable<T> GetNext(int id, int count, string query);
    IAsyncEnumerable<T> GetPrevious(int id, int count, string query);
    Task<T?> FindAsync(int id);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
    Task<int> CreateAsync(T entity);
}