using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/")]
public class TenantController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(int id) =>
        Ok(await tenantService.GetItemAsync(id));

    [HttpGet]
    public async Task<ActionResult<List<TenantModel>>> ListTenants(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) => 
        Ok((await tenantService.ListItemsAsync(limit, offset, search)).ToList());

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] TenantModel model) =>
        await tenantService.CreateItemAsync(model) ? Ok() : BadRequest();
    
}