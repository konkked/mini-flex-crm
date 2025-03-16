using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public class UserService(IUserRepo repo) : TenantBoundBaseService<UserDbModel, UserModel>(repo), IUserService
{
    protected override UserModel DbModelToApiModel(UserDbModel model) 
        => Converter.From(model);
    
    protected override UserDbModel ApiModelToDbModel(UserModel model) 
        => Converter.To(model);
    
    public async Task<bool> TryEnableAsync(int callerTenant, int userId)
    {
        var user = await repo.FindAsync(userId).ConfigureAwait(false);
        if (user == null || (callerTenant != user.TenantId && callerTenant != 0) || user.Enabled == true)
            return false;
        user.Enabled = true;
        await repo.UpdateAsync(user).ConfigureAwait(false);
        return true;
    }
    
    public async Task<bool> TryDisableAsync(int callerTenant, int userId)
    {
        var user = await repo.FindAsync(userId).ConfigureAwait(false);
        if (user == null || (callerTenant != user.TenantId && callerTenant != 0) || user.Enabled == false)
            return false;
        user.Enabled = false;
        await repo.UpdateAsync(user).ConfigureAwait(false);
        return true;
    }
}