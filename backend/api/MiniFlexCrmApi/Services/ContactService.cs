using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class ContactService(IContactRepo repo, IEntityContactRepo linkRepo) : TenantBoundBaseService<ContactDbModel, ContactModel>(repo), IContactService
{
    protected override ContactModel DbModelToApiModel(ContactDbModel model) => Converter.From(model);
    protected override ContactDbModel ApiModelToDbModel(ContactModel model) => Converter.To(model);
    
    public async Task<int> UpsertWithLinks(ContactModel contact)
    {
        // Update the Contact entry
        var id = await UpsertAsync(contact).ConfigureAwait(false);
        var updated = id != -1;
        contact.Id = id;
        if (!updated)
            return contact.Id;

        if (contact is { EntityName: not null, EntityId: not null })
        {
            // Delete existing EntityContact entries for this Contact
            var existingLinks = linkRepo.GetSomeAsync(int.MaxValue, 0,
                new Dictionary<string, object> { { "contact_id", contact.Id } });
            updated = false;
            await foreach (var link in existingLinks)
            {
                if (link.ContactId == contact.Id 
                    && link.EntityName == contact.EntityName 
                    && link.EntityId == contact.EntityId)
                {
                    link.SignificanceOrdinal = contact.SignificanceOrdinal ?? 1;
                    await linkRepo.UpdateAsync(link).ConfigureAwait(false);
                    updated = true;
                    break;
                }
            }
            
            //did not find matching link
            if (!updated)
                await linkRepo.CreateAsync(Converter.ToLink(contact)).ConfigureAwait(false);
        }

        return contact.Id;
    }
}