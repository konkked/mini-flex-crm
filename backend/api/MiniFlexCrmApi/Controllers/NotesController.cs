using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

[ApiController]
[Route("api/tenant/{tenantId}/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;

    public NotesController(INotesService notesService)
    {
        _notesService = notesService;
    }

    [HttpGet("{*route}")]
    public async Task<ActionResult<IEnumerable<NoteModel>>> GetNotes([FromRoute] int tenantId, 
        [FromRequestContext] RequestContext context, 
        [FromRoute] string route)
    {
        var userId = int.Parse(context.UserId);
        var notes = await _notesService.Get5LatestNotes(userId, tenantId, route);
        return Ok(notes);
    }

    [HttpPost("{*route}")]
    public async Task<ActionResult<NoteModel>> CreateNote([FromRoute] int tenantId, 
        [FromRequestContext] RequestContext context, 
        [FromBody] NoteModel note,
        [FromRoute] string route)
    {
        var userId = int.Parse(context.UserId);
        note.UserId = userId;
        note.TenantId = tenantId;
        note.Route = route;
        var id = await _notesService.CreateAsync(note);
        note.Id = id;
        return CreatedAtAction(nameof(GetNotes), new { route }, note);
    }
    
    [HttpPut("{*route}")]
    public async Task<ActionResult<NoteModel>> UpdateNote([FromRoute] int tenantId, 
        [FromRequestContext] RequestContext context, 
        [FromBody] NoteModel note)
    {
        var userId = int.Parse(context.UserId);
        note.UserId = userId;
        note.TenantId = tenantId;
        var id = await _notesService.CreateAsync(note);
        note.Id = id;
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote([FromRequestContext] RequestContext context, int id)
    {
        var userId = int.Parse(context.UserId);
        await _notesService.DeleteAsync(id, userId);
        return NoContent();
    }

    [HttpPut("{id}/pin")]
    public async Task<IActionResult> PinNote([FromRequestContext] RequestContext context, [FromRoute] int id)
    {
        var userId = int.Parse(context.UserId);
        await _notesService.TogglePinAsync(id, userId);
        return NoContent();
    }
}
