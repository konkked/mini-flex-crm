using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[Route("api/tenant/{tenantId}/lead")]
[ApiController]
public class LeadController(ILeadService service) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int tenantId, int id)
    {
        var result = await service.GetAsync(tenantId, id);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> List(int tenantId, [FromQuery] int limit = ServiceContants.PageSize, [FromQuery] int offset = 0)
    {
        var result = await service.ListAsync(limit, offset, new Dictionary<string, object> { { "tenant_id", tenantId } });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int tenantId, [FromBody] LeadModel model)
    {
        model.TenantId = tenantId;
        var id = await service.CreateAsync(model);
        return CreatedAtAction(nameof(Get), new { tenantId, id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int tenantId, int id, [FromBody] LeadModel model)
    {
        model.Id = id;
        model.TenantId = tenantId;
        var result = await service.UpdateAsync(model);
        return result ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int tenantId, int id)
    {
        var result = await service.DeleteAsync(tenantId, id);
        return result ? Ok() : NotFound();
    }
}