namespace MiniFlexCrmApi.Api.Services;

public interface IBaseReaderService<TApiModel>
{
    Task<TApiModel> GetItem(int id);
    Task<Page<TApiModel>> ListItems();
    Task<Page<TApiModel>> ListItems(int pageSize);
    Task<Page<TApiModel>> ListItems(int pageSize, string? next);
}