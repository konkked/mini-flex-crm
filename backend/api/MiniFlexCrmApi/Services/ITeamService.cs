using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Models;

namespace MiniFlexCrmApi.Services;

public interface ITeamService : ITenantBoundBaseService<TeamModel>
{
    Task<IEnumerable<TeamMemberModel>> NewMemberSearchAsync(int id, string name);
    Task<IEnumerable<UserModel>> NewOwnerSearchAsync(int id, string name);
    Task<bool> AddMemberAsync(int teamId, int userId, string role);
    Task<bool> RemoveMemberAsync(int teamId, int memberId);
    Task<bool> AddAccountAsync(int teamId, int accountId);
    Task<bool> RemoveAccountAsync(int teamId, int accountId);
    Task<bool> UpdateOwnerAsync(int teamId, int ownerId);
    Task<bool> UpdateNameAsync(int id, string name);
    Task<bool> UpdateInfoAsync(int id, BaseTeamModel model);
   Task<IEnumerable<TeamModel>> GetMyTeamsAsync();
}

public class TeamService(ITeamRepo repo, IUserRepo userRepo)
    : TenantBoundBaseService<TeamDbModel, TeamModel>(repo), ITeamService
{
    protected override TeamModel DbModelToApiModel(TeamDbModel model)=>Converter.From(model);

    protected override TeamDbModel ApiModelToDbModel(TeamModel model) => Converter.To(model);

    public async Task<IEnumerable<TeamMemberModel>> NewMemberSearchAsync(int id, string name)
    {
        var result = await repo.GetPossibleMembersAsync(id, name);
        return result.SelectMany(Converter.ToTeamMemberModels);
    }

    public async Task<IEnumerable<UserModel>> NewOwnerSearchAsync(int id, string name)
    {
        var result = await repo.GetPossibleOwnersAsync(id, name);
        return result.Select(Converter.From);
    }

    public async Task<IEnumerable<TeamModel>> GetMyTeamsAsync()
    {
        var result = await repo.GetMyTeams();
        return result.Select(Converter.From);
    }

    public async Task<bool> AddMemberAsync(int teamId, int memberId, string role)
    {
        try
        {
            await repo.AddMemberAsync(teamId, memberId, role);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveMemberAsync(int teamId, int memberId)
    {
        try
        {
            return await repo.RemoveMemberAsync(teamId, memberId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public Task<bool> AddAccountAsync(int teamId, int accountId)
    {
        return repo.AddAccountAsync(teamId, accountId);
    }

    public Task<bool> RemoveAccountAsync(int teamId, int accountId)
    {
        return repo.RemoveAccountAsync(teamId, accountId);
    }

    public Task<bool> UpdateOwnerAsync(int teamId, int ownerId)
    {
        return repo.UpdateOwnerAsync(teamId, ownerId);
    }

    public Task<bool> UpdateNameAsync(int id, string name)
    {
        return repo.UpdateNameAsync(id, name);
    }

    public Task<bool> UpdateInfoAsync(int id, BaseTeamModel model)
    {
        return repo.UpdateTeamInfoAsync(id, model.Name, model.Owner.Id, model.Attributes);
    }
}