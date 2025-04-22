using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IContactService : ITenantBoundBaseService<ContactModel>
{
    Task<int> UpsertWithLinksAsync(ContactModel contact);
    IAsyncEnumerable<ContactModel> GetForAsync(string entity, int entityId);
}