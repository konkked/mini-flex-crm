using System.Threading.Tasks;

namespace MiniFlexCrmApi.Api.Services;

public interface IBaseService<TApiModel> : IBaseReaderService<TApiModel> where TApiModel : BaseApiModel
{
    Task<bool> DeleteItem(int id);
    Task<bool> CreateItem(TApiModel model);
    Task<bool> UpdateItem(TApiModel model);
}