using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/team")]
public class TeamController(ITeamService teamService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountModel>> Get(
        [FromRoute] int tenantId, [FromRoute] int id) =>
        Ok(await teamService.GetAsync(tenantId, id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamMemberModel>>> List(
        [FromRoute] int tenantId,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null)
        => Ok(await teamService.ListAsync(limit, offset, search,
            parameters: new Dictionary<string, object> { ["tenant_id"] = tenantId }));

    [HttpGet]
    [Route("{id}/potential_members")]
    public async Task<ActionResult<IEnumerable<UserModel>>> SearchPotentialMembers([FromRoute] int id, [FromQuery] string name)
        => Ok(await teamService.NewMemberSearchAsync(id, name));
    
    [HttpPut]
    [Route("{id}/member")]
    public async Task<ActionResult> AddTeamMember([FromRoute] int id, [FromBody] TeamMemberModel teamMember)
        => Ok(await teamService.AddMemberAsync(id, teamMember.User.Id, teamMember.Role));
    
    [HttpDelete]
    [Route("{id}/member/{memberId}")]
    public async Task<ActionResult> RemoveTeamMember([FromRoute] int id, [FromRoute] int memberId)
        => Ok(await teamService.RemoveMemberAsync(id, memberId));
    
    [HttpPut]
    [Route("{id}/owner/{ownerId}")]
    public async Task<ActionResult> UpdateOwner([FromRoute] int id, [FromRoute] int ownerId)
        => Ok(await teamService.UpdateOwnerAsync(id, ownerId));
    
    [HttpPut]
    [Route("{id}/account")]
    public async Task<ActionResult> AddAccount([FromRoute] int id, [FromBody] AccountModel account)
        => Ok(await teamService.AddAccountAsync(id, account.Id));
    
    [HttpDelete]
    [Route("{id}/account/{accountId}")]
    public async Task<ActionResult> RemoveAccount([FromRoute] int id, [FromRoute] int accountId)
        => Ok(await teamService.RemoveAccountAsync(id, accountId));
    

    [HttpPost]
    public async Task<IActionResult> Create([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId,  [FromBody] TeamModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(context.TenantId != 0)
            return Forbid();
        return Ok(await teamService.CreateAsync(model).ConfigureAwait(false));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> Update([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId, 
        [FromRoute] int id, [FromBody] TeamModel model)
    {
        model.Id = id;
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(context.TenantId != 0)
            return Forbid();
        return await teamService.UpdateAsync(model) ? Ok() : NotFound();
    }

    [HttpPut("{id}/info")]
    public async Task<ActionResult<bool>> UpdateTeamInfo([FromRoute] int id, [FromBody] BaseTeamModel model)
    {
        return Ok(await teamService.UpdateInfoAsync(id, model));
    }

    [HttpPut("{id}/name")]
    public async Task<ActionResult<bool>> UpdateName([FromRoute] int id, [FromBody] string name)
    {
        return Ok(await teamService.UpdateNameAsync(id, name));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] int tenantId, [FromRoute] int id) =>
        await teamService.DeleteAsync(tenantId, id) ? Ok() : NotFound();
}