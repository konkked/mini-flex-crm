using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Context;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Security;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id) =>
        Ok(await userService.GetItem(id));

    [HttpGet]
    public async Task<IActionResult> ListUsers(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) => 
        Ok(await userService.ListItems(limit, offset, search));

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserModel model) =>
        await userService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel model)
    {
        model.Id = id;
        return await userService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id) =>
        await userService.DeleteItem(id) ? Ok() : NotFound();
    
    [HttpPost("{id}/enable")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> Enable(
        [FromRequestContext] RequestContext requestContext,
        [FromRoute] int id) => 
        Ok(await userService.TryEnableUserAsync(requestContext.TenantId ?? -1, id)
            .ConfigureAwait(false));
    
    [HttpPost("{id}/disable")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> Disable(
        [FromRequestContext] RequestContext requestContext,
        [FromRoute] int id) => 
        Ok(await userService.TryDisableUserAsync(requestContext.TenantId ?? -1, id)
            .ConfigureAwait(false));

}