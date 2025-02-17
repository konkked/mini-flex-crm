using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;

namespace MiniFlexCrmApi.Api.Services;

public class CustomerService(ICustomerRepo repo, IRelationRepo relationRepo) : BaseService<CustomerDbModel, CustomerModel>(repo), ICustomerService
{
    protected override CustomerModel ConvertToApiModel(CustomerDbModel model)=>Converter.From(model, null);

    protected override CustomerDbModel ConvertToDbModel(CustomerModel model) => Converter.To(model);

    public override async Task<CustomerModel> GetItem(int id)
    {
        var customer = await repo.FindAsync(id).ConfigureAwait(false);
        if (customer == null) return null;
        var relations= await relationRepo.GetCustomerRelationshipsAsync(id).ConfigureAwait(false);
        return Converter.From(customer, relations);
    }

    public async Task<CustomerModel> GetCustomerWithRelationship(int customerId)
    {
        var customer = repo.FindAsync(customerId);
        var relationships = relationRepo.GetCustomerRelationshipsAsync(customerId);
        await Task.WhenAll(customer, relationships).ConfigureAwait(false);
        return Converter.From(await customer.ConfigureAwait(false), 
            await relationships.ConfigureAwait(false));
    }
}