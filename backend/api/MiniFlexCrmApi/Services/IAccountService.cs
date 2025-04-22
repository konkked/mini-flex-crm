using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IAccountService : ITenantBoundBaseService<AccountModel>
{
    Task<AccountModel> GetWithRelationship(int accountId);
}