using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Security;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/")]
public class TenantController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("{id}")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> GetTenant(int id) =>
        Ok(await tenantService.GetAsync(id));

    [HttpGet]
    [AuthorizeRoles("admin")]
    public async Task<ActionResult<List<TenantModel>>> ListTenants(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) => 
        Ok((await tenantService.ListAsync(limit, offset, search)).ToList());

    [HttpPost]
    [AuthorizeRoles("admin")]
    public async Task<ActionResult<TenantModel>> CreateTenant([FromBody] TenantModel tenant)
    {
        var id = await tenantService.CreateAsync(tenant);
        return CreatedAtAction(nameof(GetTenant), new { id });
    }

    [HttpPut("{id}")]
    [AuthorizeRoles("admin")]
    public async Task<IActionResult> UpdateTenant(int id, [FromBody] TenantModel tenant)
    {
        tenant.Id = id;
        await tenantService.UpdateAsync(tenant);
        return NoContent();
    }
    
    [HttpGet("{id}/mine")]
    public async Task<IActionResult> GetTenant([FromRequestContext] RequestContext context, [FromRoute] int id) =>
        context.TenantId != id ? Forbid() : Ok(await tenantService.GetAsync(id));
}