using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class CustomerService(ICustomerRepo repo, IRelationshipRepo relationshipRepo) : 
    TenantBoundBaseService<CustomerDbModel, CustomerModel>(repo), ICustomerService
{
    protected override CustomerModel DbModelToApiModel(CustomerDbModel model) 
        => Converter.From(model, null);

    protected override CustomerDbModel ApiModelToDbModel(CustomerModel model) 
        => Converter.To(model);

    public override async Task<CustomerModel?> GetAsync(int id)
    {
        var customer = await repo.FindAsync(id).ConfigureAwait(false);
        if (customer is null) 
            return null;
        var relations= await relationshipRepo.GetCustomerRelationshipsAsync(id).ConfigureAwait(false);
        return Converter.From(customer, relations);
    }

    public async Task<CustomerModel> GetCustomerWithRelationship(int tenantId, int customerId)
    {
        var customer = repo.FindInTenantById(customerId, tenantId);
        var relationships = relationshipRepo.GetCustomerRelationshipsAsync(customerId);
        await Task.WhenAll(customer, relationships).ConfigureAwait(false);
        return Converter.From(await customer.ConfigureAwait(false), 
            await relationships.ConfigureAwait(false));
    }
}