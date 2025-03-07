namespace MiniFlexCrmApi.Services;

public interface ITenantBoundBaseService<TApiModel> : IBaseService<TApiModel> 
    where TApiModel : BaseApiModel
{
    Task<TApiModel> GetItemAsync(int tenantId, int id);
    Task<bool> DeleteItemAsync(int tenantId, int id);
}