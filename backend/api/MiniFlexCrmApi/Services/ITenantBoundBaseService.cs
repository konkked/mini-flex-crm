using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface ITenantBoundBaseService<TApiModel> : IBaseService<TApiModel> 
    where TApiModel : BaseApiModel
{
    Task<TApiModel> GetAsync(int tenantId, int id);
    Task<bool> DeleteAsync(int tenantId, int id);
}