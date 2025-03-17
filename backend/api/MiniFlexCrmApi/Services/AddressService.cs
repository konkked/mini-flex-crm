using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class AddressService(IAddressRepo repo, IEntityAddressRepo linkRepo) : TenantBoundBaseService<AddressDbModel, AddressModel>(repo), IAddressService
{
    protected override AddressModel DbModelToApiModel(AddressDbModel model) => Converter.From(model);
    protected override AddressDbModel ApiModelToDbModel(AddressModel model) => Converter.To(model);
    
    
    public async Task<int> UpsertWithLinks(AddressModel address)
    {
        // Update the Contact entry
        var id = await UpsertAsync(address).ConfigureAwait(false);
        var updated = id != -1;
        address.Id = id;
        if (!updated)
            return address.Id;

        if (address is { EntityName: not null, EntityId: not null })
        {
            // Delete existing EntityContact entries for this Contact
            var existingLinks = linkRepo.GetSomeAsync(int.MaxValue, 0,
                new Dictionary<string, object> { { "contact_id", address.Id } });
            updated = false;
            await foreach (var link in existingLinks)
            {
                if (link.AddressId == address.Id 
                    && link.EntityName == address.EntityName 
                    && link.EntityId == address.EntityId)
                {
                    link.SignificanceOrdinal = address.SignificanceOrdinal ?? 1;
                    await linkRepo.UpdateAsync(link).ConfigureAwait(false);
                    updated = true;
                    break;
                }
            }
            
            //did not find matching link
            if (!updated)
                await linkRepo.CreateAsync(Converter.ToLink(address)).ConfigureAwait(false);
        }

        return address.Id;
    }
}