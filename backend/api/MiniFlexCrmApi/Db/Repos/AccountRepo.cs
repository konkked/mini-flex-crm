using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using MiniFlexCrmApi.Db.Models;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Util;

namespace MiniFlexCrmApi.Db.Repos;

public class AccountRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<AccountDbModel>(connectionProvider, context), IAccountRepo;


public class TeamRepo(IConnectionProvider connectionProvider, RequestContext context)
    : TenantBoundDbEntityRepo<TeamDbModel>(connectionProvider, context), ITeamRepo
{
    private readonly RequestContext _context = context;

    private static ImmutableList<TeamDbModel> CreateTeamsFromQueryResults(IList<dynamic> results)
    {
        var teams = new Dictionary<int, TeamDbModel>();
        var members = new Dictionary<int,List<TeamMemberDbModel>>();
        foreach (var teamMember in results)
        {
            var teamId = (int)teamMember.team_id;
            var team = teams.GetValueOrDefault(teamId, new TeamDbModel());
            teams[teamId] = team;
            var teamMembers = members.GetValueOrDefault(teamId, []);

            // assigning these multiple times isnt expensive so dont even check just do it
            team.Id = teamMember.team_id;
            team.TenantId = teamMember.tenant_id;
            team.TenantName = teamMember.tenant_name;
            team.Name = teamMember.team_name;
            team.Attributes = teamMember.team_attributes;
            
            // this will be repeated over and over no need to create new object every time
            team.Owner ??= new UserDbModel
            {
                Id = teamMember.owner_id,
                Name = teamMember.owner_name,
                Role = teamMember.owner_role,
                Email = teamMember.owner_email,
                Enabled = teamMember.owner_enabled,
                ProfileImage = teamMember.owner_profile_image,
                Attributes = teamMember.owner_attributes,
            };

            teamMembers.Add(new TeamMemberDbModel
            {
                //role in the team is not the system role
                Role = teamMember.team_member_type,
                User = new UserDbModel
                {
                    Id = teamMember.id,
                    Name = teamMember.name,
                    Role = teamMember.role,
                    Email = teamMember.email,
                    Enabled = teamMember.enabled,
                    ProfileImage = teamMember.profile_image,
                    Attributes = teamMember.attributes,
                }
            });
        }
        
        foreach(var team in teams.Values)
            team.Members = members.GetValueOrDefault(team.Id, []).ToArray();

        return teams.Values.ToImmutableList();
    }
    
    public override IAsyncEnumerable<TeamDbModel> GetSomeAsync(int limit) => GetSomeAsync(limit, 50);
    public override IAsyncEnumerable<TeamDbModel> GetSomeAsync(int limit, int offset)=>GetSomeAsync(limit, offset, null as IDictionary<string, object>);

    public override IAsyncEnumerable<TeamDbModel> GetSomeAsync(int limit, string? query) =>
        GetSomeAsync(limit, 0, GrokQuery(query)?.values);

    public override async Task<TeamDbModel> FindAsync(int id)
    {
        bool addTenantSearch = _context.TenantId != 0;
        
        await ConnectionProvider.GetConnection().OpenAsync();
        var results = ConnectionProvider.GetConnection().QueryAsync($@"
            SELECT t.id as team_id, 
                   tn.name as tenant_name, 
                   t.name as team_name, 
                   t.owner_id,
                   m.team_member_type,
                   u.id as user_id,
                   u.name as user_name,
                   u.email as user_email,
                   u.profile_image as user_profile_image,
                   u.enabled as user_enabled,
                   u.attributes as user_attributes,
                   owner.id as owner_id,
                   owner.name as owner_name,
                   owner.email as owner_email,
                   owner.profile_image as owner_profile_image,
                   owner.enabled as owner_enabled,
                   owner.attributes as owner_attributes,
                   t.attributes as team_attributes 
            FROM TeamMember m 
                    JOIN Team t 
                          ON m.team_id = t.id
                    JOIN app_user u on u.id = m.user_id
                    JOIN app_user owner on owner.id = t.owner_id
                    JOIN tenant tn on tn.id = t.tenant_id
            WHERE t.id = @id "+(addTenantSearch ? $"AND t.tenant_id = @tenantId" : ""), new { id, tenantId = _context.TenantId});
        var resultsLs = (await results.ConfigureAwait(false)).ToList();
        return CreateTeamsFromQueryResults(resultsLs).Single();
    }

    public override async IAsyncEnumerable<TeamDbModel> GetSomeAsync(int limit, int offset, IDictionary<string, object>? parameters)
    {
        var parmaetersClone = new Dictionary<string, object>(parameters ?? new Dictionary<string, object>());
        parameters = parmaetersClone;
        var hasNameFilter = parameters?.TryGetValue("name", out var name) ?? false;
        var tenantIdFilter = parameters?.TryGetValue("tenantId", out var tenantId) ?? false;
        if (!tenantIdFilter && _context.TenantId.HasValue && _context.TenantId != 0)
            parameters["tenantId"] = _context.TenantId;
        parameters["limit"] = limit;
        parameters["offset"] = offset;
        await ConnectionProvider.GetConnection().OpenAsync();
        var results = await ConnectionProvider.GetConnection().QueryAsync($@"
            SELECT t.id as team_id, t.tenant_id, tn.name as tenant_name, t.name as team_name, t.owner_id,m.team_member_type,u.*,t.attributes as team_attributes 
            FROM team_member m 
                    JOIN (SELECT * FROM team {(hasNameFilter ? "WHERE name LIKE CONCAT('%', @name, '%')" : "" )} LIMIT @limit OFFSET @offset) t 
                          ON m.team_id = t.id
                    JOIN app_user u on u.id = m.user_id
                    JOIN tenant tn on tn.id = t.tenant_id
            WHERE (t.tenant_id = @tenantId OR @tenantId = 0)
            ORDER BY t.id", parameters);
        var resultsLs = results.ToList();
        var teamDbModels = CreateTeamsFromQueryResults(resultsLs);
        
        var teamIds = resultsLs.Select(x => (int)x.team_id);
        
        var accounts = await ConnectionProvider.GetConnection().QueryAsync(@"
            SELECT a.*, ta.team_id, 
                   sales_rep.id as sales_rep_id, sales_rep.name as sales_rep_name, sales_rep.profile_image as sales_rep_profile_image, sales_rep.attributes as sales_rep_attributes
                   pre_sales_rep.id as pre_sales_rep_id, pre_sales_rep.name as pre_sales_rep_name, pre_sales_rep.profile_image as pre_sales_rep_profile_image, pre_sales_rep.attributes as pre_sales_rep_attributes
                   account_manager.id as account_manager_id, account_manager.name as account_manager_name, account_manager.profile_image as account_manager_profile_image, account_manager.attributes as account_manager_attributes
            FROM account a 
                JOIN team_account ta 
                    ON ta.account_id = a.id 
                JOIN app_user sales_rep ON a.sales_rep_id = sales_rep.id
                JOIN app_user pre_sales_rep ON a.pre_sales_rep_id = pre_sales_rep.id
                JOIN app_user account_manager ON a.account_manager_id = account_manager.id
            WHERE ta.team_id=ANY(@teamIds);", new { teamIds });
        
        var accountGroups = accounts.GroupBy(x=>x.team_id)
            .ToDictionary(x => x.Key);

        foreach (var team in teamDbModels)
        {
            if (accountGroups.TryGetValue(team.Id, out var teamAccounts))
            {
                team.Accounts = teamAccounts.Select(a => new AccountDbModel
                {
                    Id = a.id,
                    Attributes = a.attributes,
                    TenantId = a.tenant_id,
                    Name = a.name,
                    PreSalesRepId = a.pre_sales_rep_id,
                    PreSalesRep = new UserDbModel
                    {
                        Id = a.pre_sales_rep_id,
                        Name = a.pre_sales_rep_name,
                        ProfileImage = a.pre_sales_rep_profile_image ??
                                       DefaultProfileImageGenerator.Create(a.pre_sales_rep_name, a.pre_sales_rep_id),
                        Attributes = a.pre_sales_rep_attributes,
                    },
                    SalesRepId = a.sales_rep_id,
                    SalesRep = new UserDbModel
                    {
                        Id = a.sales_rep_id,
                        Name = a.sales_rep_name,
                        ProfileImage = a.sales_rep_profile_image ??
                                       DefaultProfileImageGenerator.Create(a.sales_rep_name, a.sales_rep_id),
                        Attributes = a.sales_rep_attributes,
                    },
                    AccountManagerId = a.account_manager_id,
                    AccountManager = new UserDbModel
                    {
                        Id = a.account_manager_id,
                        Name = a.account_manager_name,
                        ProfileImage = a.account_manager_profile_image ??
                                       DefaultProfileImageGenerator.Create(a.account_manager_name,
                                           a.account_manager_id),
                        Attributes = a.account_manager_attributes,
                    }
                }).ToArray();
            }

            yield return team;
        }
    }

    public override async Task<int> UpdateAsync(TeamDbModel entity)
    {
        await ConnectionProvider.GetConnection().OpenAsync();
        var current = await FindAsync(entity.Id);
        var updatedCount = 0;
        var updatedOwnerId = entity.Owner != null && entity.Owner.Id != current?.Owner?.Id;
        var updateTeamName = entity.Name != null && entity.Name != current?.Name;
        var updateDescription = entity.Description != null && entity.Description != current?.Description;
        var updateAttributes = !JsonSerializer.Serialize(entity.Attributes)
            .Equals(JsonSerializer.Serialize(current.Attributes), StringComparer.InvariantCultureIgnoreCase);
        
        //make sure new user is enabled
        if (updatedOwnerId)
        {
            var user = await ConnectionProvider.GetConnection()
                .QueryFirstAsync("SELECT * FROM user WHERE id=@id", new { id = entity.Owner.Id })
                .ConfigureAwait(false);

            if (!user.enabled)
            {
                throw new InvalidOperationException("Disabled user cannot be made owner of a team.");
            }
        }

        if (updatedOwnerId || updateTeamName || updateDescription || updateAttributes)
        {
            await ConnectionProvider.GetConnection().ExecuteAsync(@"
            UPDATE team 
            SET name=@name, owner_id=@ownerId, description=@description, attributes=@attributes 
            WHERE team_id=@teamId",
                new
                {
                    ownerId = updatedOwnerId ? entity.Owner?.Id : current.Owner?.Id,
                    name = updateTeamName ? entity.Name : current.Name,
                    description = updateDescription ? entity.Description : current.Description,
                    attributes = JsonSerializer.Serialize(updateAttributes ? entity.Attributes : current.Attributes),
                    teamId = entity.Id
                });
            updatedCount++;
        }

        var currentAccounts = current.Accounts;
        
        var addingAccounts = entity.Accounts
            .Where(newAccount => 
                !currentAccounts.Select(currentAccount=>currentAccount.Id)
                    .Contains(newAccount.Id))
            .ToImmutableList();
        var removingAccounts = 
            currentAccounts.Select(currentAccount => currentAccount.Id)
                .Where(currentAccount => !entity.Accounts.Select(newAccount => newAccount.Id)
                    .Contains(currentAccount))
                .ToImmutableList();
        
        var currentMembers = current.Members;
        var addingMembers = entity.Members
            .Where(newMember => 
                !currentMembers.Select(currentMember=>(userId: currentMember.User.Id, role: currentMember.Role))
                    .Contains((userId: newMember.User.Id, role: newMember.User.Role)))
            .ToImmutableList();
        var removingMembers = current.Members
            .Where(currentMember=> 
                !entity.Members.Select(newMember=>(userId: newMember.User.Id, role: newMember.Role))
                    .Contains((userId:currentMember.User.Id, role: currentMember.User.Role)))
            .ToImmutableList();
        
        if (!removingMembers.IsEmpty)
            foreach (var member in removingMembers)
            {
                await ConnectionProvider.GetConnection().ExecuteAsync(@"
                DELETE FROM team_member 
                WHERE team_id = @teamId AND user_id = @userId AND role = @role",
                    new { teamId = entity.Id, userId = member.User.Id, role = member.Role }).ConfigureAwait(false);
            }

        updatedCount += removingMembers.Count;

        if (!addingMembers.IsEmpty)
            foreach (var member in addingMembers)
            {
                await ConnectionProvider.GetConnection().ExecuteAsync(@"
                INSERT INTO team_member(team_id, user_id, role) VALUES(@teamId,@userId,@role)",
                    new { teamId = entity.Id, userId = member.User.Id, role = member.Role }).ConfigureAwait(false);
            }
        
        updatedCount += addingMembers.Count;

        if (!removingAccounts.IsEmpty)
            await ConnectionProvider.GetConnection().ExecuteAsync(@"
                DELETE FROM team_account WHERE team_id=@teamId and id=@ids", 
                new { teamId = entity.Id, ids=removingAccounts.ToList() }).ConfigureAwait(false);

        updatedCount += removingAccounts.Count;
        
        if(!addingAccounts.IsEmpty)
            await Task.WhenAll(addingAccounts.Select(id =>
                ConnectionProvider.GetConnection().ExecuteAsync(@"
                 INSERT INTO team_account(team_id, account_id) VALUES(@teamId, @accountId)",
                new { teamdId = entity.Id, accountId = id }))).ConfigureAwait(false);

        updatedCount += addingAccounts.Count;
        
        return updatedCount;
    }

    public async Task<IEnumerable<UserDbModel>> GetPossibleMembersAsync(int id, string name)
    {
        await ConnectionProvider.GetConnection().OpenAsync();
        var parameters = new Dictionary<string, object>
        {
            ["@tenantId"] = _context.TenantId,
            ["@id"] = id
        };
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            parameters.Add("@name", name);
        }

        return await ConnectionProvider.GetConnection().QueryAsync<UserDbModel>(
            @"SELECT u.* FROM users u 
                LEFT JOIN team_member t 
                    ON t.member_id = u.id AND t.team_id = @id
                WHERE t.member_id IS NULL AND u.tenant_id = @tenantId "
            + (!string.IsNullOrWhiteSpace(name) ? " AND u.name LIKE CONCAT('%',@name,'%')" : ""), parameters);
    }

    public async Task AddMemberAsync(int teamId, int memberId, string role)
    {
        await ConnectionProvider.GetConnection().OpenAsync();
        await ConnectionProvider.GetConnection().ExecuteAsync(@"
        INSERT INTO team_member(team_member_type,team_id,member_id) VALUES(@role,@teamId,@memberId)", new
        {
            role,
            teamId,
            memberId
        });
    }

    public async Task<bool> RemoveMemberAsync(int teamId, int memberId)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            @"DELETE FROM team_member WHERE team_id=@teamId AND member_id=@memberId",
            new { teamId, memberId }) == 1;
    }

    public async Task<bool> RemoveAccountAsync(int teamId, int accountId)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            @"DELETE FROM team_account WHERE team_id=@teamId AND account_id=@accountId", 
            new { teamId, accountId }).ConfigureAwait(false) == 1;
    }

    public async Task<bool> AddAccountAsync(int teamId, int accountId)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            @"INSERT INTO team_account(team_id, account_id) VALUES(@teamId, @accountId)", 
            new { teamId, accountId }).ConfigureAwait(false) == 1;
    }

    public async Task<bool> UpdateOwnerAsync(int teamId, int ownerId)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            "UPDATE team SET owner_id=@ownerId WHERE id=@teamId",
            new { teamId, ownerId }) == 1;
    }

    public async Task<bool> UpdateNameAsync(int id, string name)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            "UPDATE team SET name=@name WHERE id=@id",
            new { name, id }) == 1;
    }

    public async Task<bool> UpdateTeamInfoAsync(int id, string? name, int ownerId, dynamic attributes)
    {
        return await ConnectionProvider.GetConnection().ExecuteAsync(
            "UPDATE team SET name=@name, owner_id=@ownerId, attributes=@attributes WHERE id=@id",
            new { name, id, attributes = JsonSerializer.Serialize(attributes) }) == 1;
    }
}