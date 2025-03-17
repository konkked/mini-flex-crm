using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IAddressService : ITenantBoundBaseService<AddressModel>
{
    Task<int> UpsertWithLinks(AddressModel address);
}