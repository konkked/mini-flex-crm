using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/relation")]
public class RelationController(IRelationService relationService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRelation(int id) =>
        Ok(await relationService.GetItem(id));

    [HttpGet]
    public async Task<IActionResult> ListRelations([FromQuery] int pageSize = 50, [FromQuery] string? next = null) =>
        Ok(await relationService.ListItems(pageSize, next));

    [HttpPost]
    public async Task<IActionResult> CreateRelation([FromBody] RelationModel model) =>
        await relationService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRelation(int id, [FromBody] RelationModel model)
    {
        model.Id = id;
        return await relationService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRelation(int id) =>
        await relationService.DeleteItem(id) ? Ok() : NotFound();
}