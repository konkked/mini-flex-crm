using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models.Public;

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
}