using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Models.Public;

[ApiController]
[Route("api/tenant/")]
public class TenantController(ITenantService tenantService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(int id) =>
        Ok(await tenantService.GetItem(id));

    [HttpGet]
    public async Task<IActionResult> ListTenants([FromQuery] int pageSize = 50, [FromQuery] string? next = null) =>
        Ok(await tenantService.ListItems(pageSize, next));

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] TenantModel model) =>
        await tenantService.CreateItem(model) ? Ok() : BadRequest();
    
}