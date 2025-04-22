using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class ContactService(ITenantBoundDbEntityRepo<ContactDbModel> repo, IRepo<EntityContactDbModel> entityContactRepo) 
    : TenantBoundBaseService<ContactDbModel, ContactModel>(repo), IContactService
{
    
    protected override ContactModel DbModelToApiModel(ContactDbModel model) => Converter.From(model);

    protected override ContactDbModel ApiModelToDbModel(ContactModel model) => Converter.To(model);

    public override async Task<ContactModel?> GetAsync(int tenantId, int id)
    {
        var model = await base.GetAsync(tenantId, id).ConfigureAwait(false);
        if (model == null) return null;

        var entityContact = await entityContactRepo.QueryOneAsync(new Dictionary<string, object>
        {
            ["contact_id"] = id
        }).ConfigureAwait(false);

        if (entityContact == null) 
            return model;
        
        model.EntityId = entityContact.EntityId;
        model.EntityName = Enum.Parse<EntityNameType>(entityContact.EntityName, true);
        model.SignificanceOrdinal = entityContact.SignificanceOrdinal;

        return model;
    }

    public async Task<int> UpsertWithLinksAsync(ContactModel contact)
    {
        // Update the Contact entry
        var id = await UpsertAsync(contact).ConfigureAwait(false);
        var updated = id != -1;
        contact.Id = id;
        if (!updated)
            return contact.Id;

        if (contact is not { EntityName: not EntityNameType.unknown, EntityId: not null }) 
            return contact.Id;
        
        // Delete existing EntityContact entries for this Contact
        var existingLinks = entityContactRepo.GetSomeAsync(int.MaxValue, 0,
            new Dictionary<string, object> { { "contact_id", contact.Id } });
        updated = false;
        await foreach (var link in existingLinks)
        {
            _ = Enum.TryParse<EntityNameType>(link.EntityName, out var linkEntityName)
                || ((linkEntityName = EntityNameType.unknown) == EntityNameType.unknown);
            if (link.ContactId == contact.Id 
                && linkEntityName == contact.EntityName 
                && link.EntityId == contact.EntityId)
            {
                link.SignificanceOrdinal = contact.SignificanceOrdinal ?? 1;
                await entityContactRepo.UpdateAsync(link).ConfigureAwait(false);
                updated = true;
                break;
            }
        }
            
        //did not find matching link
        if (!updated)
            await entityContactRepo.CreateAsync(Converter.ToLink(contact)).ConfigureAwait(false);
        
        await base.UpsertAsync(contact).ConfigureAwait(false);
        
        return contact.Id;
    }

    public async IAsyncEnumerable<ContactModel> GetForAsync(string entity, int entityId)
    {
        var results = new List<EntityContactDbModel>();

        await foreach (var result in entityContactRepo.GetSomeAsync(10000, 0, new Dictionary<string, object>
                       {
                           ["entity_name"] = entity,
                           ["entity_id"] = entityId
                       }))
            results.Add(result);
        
        await foreach (var contactDbModel in repo.GetSomeAsync(10000, 0, new Dictionary<string, object>
                       {
                           ["id"]=results.Select(a=>a.ContactId).ToArray()
                       }))
            yield return DbModelToApiModel(contactDbModel);
    }

    public override async Task<bool> UpdateAsync(ContactModel model)
    {
        var success = await base.UpdateAsync(model).ConfigureAwait(false);
        if (!success || model.EntityId == null || model.EntityName == EntityNameType.unknown) return success;

        var entityContact = await entityContactRepo.QueryOneAsync(new Dictionary<string, object> {
            ["contact_id"] = model.Id
        }).ConfigureAwait(false);

        if (entityContact == null)
        {
            entityContact = Converter.ToLink(model);
            await entityContactRepo.CreateAsync(entityContact).ConfigureAwait(false);
        }
        else
        {
            entityContact.EntityId = model.EntityId.Value;
            entityContact.EntityName = model.EntityName.ToString();
            entityContact.SignificanceOrdinal = model.SignificanceOrdinal ?? 1;
            await entityContactRepo.UpdateAsync(entityContact).ConfigureAwait(false);
        }

        return true;
    }
}