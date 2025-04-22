using MiniFlexCrmApi.Db.Models;

namespace MiniFlexCrmApi.Db.Repos;

public interface IAccountRepo : ITenantBoundDbEntityRepo<AccountDbModel>;

public interface ITeamRepo : ITenantBoundDbEntityRepo<TeamDbModel>
{
    Task<IEnumerable<UserDbModel>> GetPossibleMembersAsync(int id, string name);
    Task AddMemberAsync(int teamId, int memberId, string role);
    Task<bool> RemoveMemberAsync(int teamId, int memberId);
    Task<bool> RemoveAccountAsync(int teamId, int accountId);
    Task<bool> AddAccountAsync(int teamId, int accountId);
    Task<bool> UpdateOwnerAsync(int teamId, int ownerId);
    Task<bool> UpdateNameAsync(int id, string name);
    Task<bool> UpdateTeamInfoAsync(int id, string? modelName, int modelOwnerId, dynamic modelAttributes);
}