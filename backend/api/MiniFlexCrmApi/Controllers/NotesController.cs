using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;
using Org.BouncyCastle.Asn1.Ocsp;

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
        var notes = await _notesService.ListAsync(userId, tenantId, route);
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
        var id = await _notesService.CreateAsync(note);
        note.Id = id;
        return CreatedAtAction(nameof(GetNotes), new { route }, note);
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
