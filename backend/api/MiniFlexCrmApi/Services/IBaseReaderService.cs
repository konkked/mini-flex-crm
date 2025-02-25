using System.Threading.Tasks;

namespace MiniFlexCrmApi.Api.Services;

public interface IBaseReaderService<TApiModel>
{
    Task<TApiModel> GetItem(int id);
    Task<IEnumerable<TApiModel>> ListItems();
    Task<IEnumerable<TApiModel>> ListItems(int limit);
    Task<IEnumerable<TApiModel>> ListItems(int limit, int offset);
    Task<IEnumerable<TApiModel>> ListItems(int limit, int offset, string? query);
}