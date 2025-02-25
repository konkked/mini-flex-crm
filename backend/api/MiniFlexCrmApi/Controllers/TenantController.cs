using System.Threading.Tasks;
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
    public async Task<IActionResult> ListTenants(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) => 
        Ok(await tenantService.ListItems(limit, offset, search));

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] TenantModel model) =>
        await tenantService.CreateItem(model) ? Ok() : BadRequest();
    
}