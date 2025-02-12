using MiniFlexCrmApi.Api.Models;

namespace MiniFlexCrmApi.Api.Services;

public interface ICustomerService : IBaseService<CustomerModel>
{
    Task<CustomerModel> GetCustomerWithRelationship(int customerId);
}