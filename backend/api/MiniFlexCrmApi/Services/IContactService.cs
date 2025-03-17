using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface IContactService : ITenantBoundBaseService<ContactModel>
{
    Task<int> UpsertWithLinks(ContactModel contact);
}