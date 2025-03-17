using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Security;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] int tenantId, [FromRoute]int id)
    {
        var user =  await userService.GetAsync(tenantId, id).ConfigureAwait(false);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> ListUsers(
        [FromRoute] int tenantId,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) => 
        Ok(await userService.ListAsync(limit, offset, search, new Dictionary<string,object>{ ["tenant_id"] = tenantId }).ConfigureAwait(false));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRequestContext] RequestContext context, [FromRoute] int tenantId, [FromRoute]int id, [FromBody] UserModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if (context.TenantId != 0)
            return Forbid();
        
        model.TenantId = tenantId;
        model.Id = id;
        return await userService.UpdateAsync(model).ConfigureAwait(false) 
            ? Ok() 
            : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteUser([FromRoute] int tenantId, [FromRoute] int id) =>
        await userService.DeleteAsync(tenantId, id).ConfigureAwait(false) 
            ? Ok() 
            : NotFound();
    
    [HttpPost("{id}/enable")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> Enable(
        [FromRequestContext] RequestContext requestContext,
        [FromRoute] int id) => 
        Ok(await userService.TryEnableAsync(requestContext.TenantId ?? -1, id)
            .ConfigureAwait(false));
    
    [HttpPost("{id}/disable")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> Disable(
        [FromRequestContext] RequestContext requestContext,
        [FromRoute] int id) => 
        Ok(await userService.TryDisableAsync(requestContext.TenantId ?? -1, id)
            .ConfigureAwait(false));

}