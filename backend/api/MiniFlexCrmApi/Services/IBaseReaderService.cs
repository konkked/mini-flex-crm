namespace MiniFlexCrmApi.Services;

public interface IBaseReaderService<TApiModel>
{
    Task<TApiModel> GetItemAsync(int id);
    Task<IEnumerable<TApiModel>> ListItemsAsync();
    Task<IEnumerable<TApiModel>> ListItemsAsync(int limit);
    Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset);
    Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset, string? query);
    Task<IEnumerable<TApiModel>> ListItemsAsync(int limit, int offset, string? query, IDictionary<string, object>? parameters);
}