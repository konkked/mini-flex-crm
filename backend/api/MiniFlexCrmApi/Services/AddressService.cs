using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;
using System.Linq;

namespace MiniFlexCrmApi.Services;

public class AddressService : TenantBoundBaseService<AddressDbModel, AddressModel>, IAddressService
{
    private readonly IRepo<EntityAddressDbModel> _entityAddressRepo;
    
    protected override AddressModel DbModelToApiModel(AddressDbModel model) => Converter.From(model);
    
    protected override AddressDbModel ApiModelToDbModel(AddressModel model) => Converter.To(model);

    public AddressService(ITenantBoundDbEntityRepo<AddressDbModel> repo, IRepo<EntityAddressDbModel> entityAddressRepo) : base(repo)
    {
        _entityAddressRepo = entityAddressRepo;
    }

    public override async Task<AddressModel?> GetAsync(int tenantId, int id)
    {
        var model = await base.GetAsync(tenantId, id).ConfigureAwait(false);
        if (model == null) return null;

        model.EntityName = EntityNameType.unknown;
        var entityAddress = await _entityAddressRepo.FindAsync(id).ConfigureAwait(false);

        if (entityAddress != null)
        {
            model.EntityId = entityAddress.EntityId;
            if (!string.IsNullOrEmpty(entityAddress.EntityName))
            {
                model.EntityName = Enum.Parse<EntityNameType>(entityAddress.EntityName, true);
            }
            model.SignificanceOrdinal = entityAddress.SignificanceOrdinal;
        }

        return model;
    }

    public override async Task<bool> UpdateAsync(AddressModel model)
    {
        var success = await base.UpdateAsync(model).ConfigureAwait(false);
        if (!success || model.EntityId == null || model.EntityName == EntityNameType.unknown) return success;

        var entityAddress = await _entityAddressRepo.FindAsync(model.Id).ConfigureAwait(false);

        if (entityAddress == null)
        {
            entityAddress = Converter.ToLink(model);
            await _entityAddressRepo.CreateAsync(entityAddress).ConfigureAwait(false);
        }
        else
        {
            entityAddress.EntityId = model.EntityId.Value;
            entityAddress.EntityName = model.EntityName.ToString();
            entityAddress.SignificanceOrdinal = model.SignificanceOrdinal ?? 1;
            await _entityAddressRepo.UpdateAsync(entityAddress).ConfigureAwait(false);
        }

        return true;
    }

    
    public async Task<int> UpsertWithLinks(AddressModel address)
    {
        // Update the Contact entry
        var id = await UpsertAsync(address).ConfigureAwait(false);
        var updated = id != -1;
        address.Id = id;
        if (!updated)
            return address.Id;

        if (address is not { EntityName: not EntityNameType.unknown, EntityId: not null }) 
            return address.Id;
        
        // Delete existing EntityContact entries for this Contact
        var existingLinks = _entityAddressRepo.GetSomeAsync(int.MaxValue, 0,
            new Dictionary<string, object> { { "contact_id", address.Id } });
        updated = false;
        await foreach (var link in existingLinks)
        {
            _ = Enum.TryParse<EntityNameType>(link.EntityName, out var linkEntityName)
                || ((linkEntityName = EntityNameType.unknown) == EntityNameType.unknown);
            if (link.AddressId == address.Id 
                && linkEntityName == address.EntityName 
                && link.EntityId == address.EntityId)
            {
                link.SignificanceOrdinal = address.SignificanceOrdinal ?? 1;
                await _entityAddressRepo.UpdateAsync(link).ConfigureAwait(false);
                updated = true;
                break;
            }
        }
            
        //did not find matching link
        if (!updated)
            await _entityAddressRepo.CreateAsync(Converter.ToLink(address)).ConfigureAwait(false);

        return address.Id;
    }
}