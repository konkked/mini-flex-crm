using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class AccountService(IAccountRepo repo, IRelationshipRepo relationshipRepo, IUserRepo userRepo) : 
    TenantBoundBaseService<AccountDbModel, AccountModel>(repo), IAccountService
{
    protected override AccountModel DbModelToApiModel(AccountDbModel model) 
        => Converter.From(model, null);

    protected override AccountDbModel ApiModelToDbModel(AccountModel model) 
        => Converter.To(model);

    public override async Task<AccountModel?> GetAsync(int id)
    {
        var account = await repo.FindAsync(id).ConfigureAwait(false);
        
        if (account is null) 
            return null;
        
        if (account.AccountManagerId != null)
            account.AccountManager = await userRepo.FindAsync(account.AccountManagerId.Value).ConfigureAwait(false);
        
        if (account.SalesRepId != null)
            account.SalesRep = await userRepo.FindAsync(account.SalesRepId.Value).ConfigureAwait(false);
        
        if (account.PreSalesRepId != null)
            account.PreSalesRep = await userRepo.FindAsync(account.PreSalesRepId.Value).ConfigureAwait(false); 
        
        return Converter.From(account);
    }

    public async Task<AccountModel> GetWithRelationship(int accountId)
    {
        var account = await repo.FindBound(accountId).ConfigureAwait(false);
        
        if (account is null) 
            return null;
        
        if (account.AccountManagerId != null)
            account.AccountManager = await userRepo.FindAsync(account.AccountManagerId.Value).ConfigureAwait(false);
        
        if (account.SalesRepId != null)
            account.SalesRep = await userRepo.FindAsync(account.SalesRepId.Value).ConfigureAwait(false);
        
        if (account.PreSalesRepId != null)
            account.PreSalesRep = await userRepo.FindAsync(account.PreSalesRepId.Value).ConfigureAwait(false);
        
        var relationships = 
            await relationshipRepo.GetAccountRelationshipsAsync(accountId).ConfigureAwait(false);
        return Converter.From(account, relationships);
    }
}