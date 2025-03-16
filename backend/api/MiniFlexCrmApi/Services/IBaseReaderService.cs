namespace MiniFlexCrmApi.Services;

public interface IBaseReaderService<TApiModel>
{
    Task<TApiModel?> GetAsync(int id);
    Task<IEnumerable<TApiModel>> ListAsync();
    Task<IEnumerable<TApiModel>> ListAsync(int limit);
    Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset);
    Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset, string? query);
    Task<IEnumerable<TApiModel>> ListAsync(int limit, int offset, string? query, IDictionary<string, object>? parameters);
}