using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/relation")]
public class RelationshipController(IRelationshipService relationshipService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<IActionResult>> GetRelationship(int id) =>
        Ok(await relationshipService.GetAsync(id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelationshipModel>>> ListRelationships(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) =>
        Ok(await relationshipService.ListAsync(limit, offset, search));

    [HttpPost]
    public async Task<ActionResult<bool>> CreateRelationship([FromBody] RelationshipModel model)
        => Ok(await relationshipService.CreateAsync(model).ConfigureAwait(false));

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateRelationship(int id, [FromBody] RelationshipModel model)
    {
        model.Id = id;
        return await relationshipService.UpdateAsync(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteRelationship(int id) =>
        await relationshipService.DeleteAsync(id) ? Ok() : NotFound();
}