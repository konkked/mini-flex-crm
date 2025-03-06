using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface ICustomerService : ITenantBoundBaseService<CustomerModel>
{
    Task<CustomerModel> GetCustomerWithRelationship(int tenantId, int customerId);
}