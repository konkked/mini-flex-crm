using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

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
    
    [HttpGet]
    public async Task<IActionResult> ListPreviousItems([FromQuery] int pageSize = 50, [FromQuery] string? prev = null) =>
        Ok(await tenantService.ListPreviousItems(pageSize, prev));

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] TenantModel model) =>
        await tenantService.CreateItem(model) ? Ok() : BadRequest();
    
}