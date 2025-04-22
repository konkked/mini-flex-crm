using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/account")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpGet("{id}/relationships")]
    public async Task<ActionResult<AccountModel>> GetAccountWithRelationships([FromRoute] int id)
        => Ok(await accountService.GetWithRelationship(id));

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountModel>> Get(
        [FromRoute] int tenantId, [FromRoute] int id) =>
        Ok(await accountService.GetAsync(tenantId, id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountModel>>> List(
        [FromRoute] int tenantId,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null)
        => Ok(await accountService.ListAsync(limit, offset, search,
            parameters: new Dictionary<string, object> { ["tenant_id"] = tenantId }));

    [HttpPost]
    public async Task<IActionResult> Create([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId,  [FromBody] AccountModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(context.TenantId != 0)
            return Forbid();
        return Ok(await accountService.CreateAsync(model).ConfigureAwait(false));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> Update([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId, 
        [FromRoute] int id, [FromBody] AccountModel model)
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
        return await accountService.UpdateAsync(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] int tenantId, [FromRoute] int id) =>
        await accountService.DeleteAsync(tenantId, id) ? Ok() : NotFound();
}