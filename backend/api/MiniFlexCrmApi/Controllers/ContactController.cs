using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[Route("api/tenant/{tenantId}/contact")]
[ApiController]
public class ContactController(IContactService service) : ControllerBase
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
        var result = await service.ListAsync(limit, offset, null, new Dictionary<string, object> { { "tenant_id", tenantId } });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int tenantId, [FromBody] ContactModel model)
    {
        model.TenantId = tenantId;
        var id = await service.UpsertWithLinks(model);
        return CreatedAtAction(nameof(Get), new { tenantId, id }, model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int tenantId, int id, [FromBody] ContactModel model)
    {
        model.Id = id;
        model.TenantId = tenantId;
        var result = await service.UpsertWithLinks(model);
        return result != -1 ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int tenantId, int id)
    {
        var result = await service.DeleteAsync(tenantId, id);
        return result ? Ok() : NotFound();
    }
}