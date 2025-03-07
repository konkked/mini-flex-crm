using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/relation")]
public class RelationshipController(IRelationService relationService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<IActionResult>> GetRelationship(int id) =>
        Ok(await relationService.GetItemAsync(id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelationshipModel>>> ListRelationships(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) =>
        Ok(await relationService.ListItemsAsync(limit, offset, search));

    [HttpPost]
    public async Task<ActionResult<bool>> CreateRelationship([FromBody] RelationshipModel model)
        => Ok(await relationService.CreateItemAsync(model).ConfigureAwait(false));

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateRelationship(int id, [FromBody] RelationshipModel model)
    {
        model.Id = id;
        return await relationService.UpdateItemAsync(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteRelationship(int id) =>
        await relationService.DeleteItemAsync(id) ? Ok() : NotFound();
}