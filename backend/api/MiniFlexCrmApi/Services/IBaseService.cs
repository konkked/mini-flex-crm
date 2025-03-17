using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IBaseService<TApiModel> : IBaseReaderService<TApiModel> where TApiModel : BaseApiModel
{
    Task<bool> DeleteAsync(int id);
    Task<int> CreateAsync(TApiModel model);
    Task<bool> UpdateAsync(TApiModel model);
    Task<int> UpsertAsync(TApiModel model);
}