using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/relation")]
public class RelationController(IRelationService relationService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<IActionResult>> GetRelationship(int id) =>
        Ok(await relationService.GetItem(id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RelationshipModel>>> ListRelationships(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null) =>
        Ok(await relationService.ListItems(limit, offset, search));

    [HttpPost]
    public async Task<ActionResult<bool>> CreateRelationship([FromBody] RelationshipModel model) =>
        await relationService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateRelationship(int id, [FromBody] RelationshipModel model)
    {
        model.Id = id;
        return await relationService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteRelationship(int id) =>
        await relationService.DeleteItem(id) ? Ok() : NotFound();
}