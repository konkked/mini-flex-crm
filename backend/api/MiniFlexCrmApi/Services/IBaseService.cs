namespace MiniFlexCrmApi.Services;

public interface IBaseService<TApiModel> : IBaseReaderService<TApiModel> where TApiModel : BaseApiModel
{
    Task<bool> DeleteItemAsync(int id);
    Task<int> CreateItemAsync(TApiModel model);
    Task<bool> UpdateItemAsync(TApiModel model);
}