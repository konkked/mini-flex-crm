namespace MiniFlexCrmApi.Db.Repos;

public interface IRepo<T> where T : class
{
    Task<bool> ExistsAsync(int id);
    IAsyncEnumerable<T> GetSome(int count);
    IAsyncEnumerable<T> GetNext(int startId, int count);
    IAsyncEnumerable<T> GetPrevious(int startId, int count);
    Task<T?> FindAsync(int id);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(int id);
    Task<int> CreateAsync(T entity);
}