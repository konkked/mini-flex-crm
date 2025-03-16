using System.Net;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers
{
    [Route("api/[controller]/tenant/{tenantId}")]
    [ApiController]
    public class AttachmentController(IAttachmentService attachmentService) : ControllerBase
    {

        [HttpPost("{entityName}/{entityId}/attachments/{name}.{ext}")]
        public async Task<IActionResult> UploadAttachment(
            [FromRoute] int tenantId, [FromRoute] string entityName, [FromRoute] int entityId, [FromRoute] string name, 
            [FromRoute] string ext, [FromForm] IFormFile file, [FromForm] string notes = null)
        {
            try
            {
                var id = await attachmentService.UploadAsync(file, name, ext, tenantId, entityName, entityId, notes); // Replace 0 with authenticated user ID
                return Ok(new { Id = id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                return Conflict("An attachment with this name already exists.");
            }
        }

        [HttpGet("{entityName}/{entityId}/attachments")]
        public async Task<IActionResult> GetAttachmentsByEntity(int tenantId, string entityName, int entityId)
        {
            try
            {
                var attachments = await attachmentService.GetAllForEntityAsync(tenantId, entityName, entityId);
                return Ok(attachments.Select(a => new { a.Id, a.Path, a.Notes, a.Ext, a.UpdatedTs }));
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("{path}")]
        public async Task<IActionResult> GetAttachment([FromRoute] int tenantId, string path)
        {
            path = $"tenant/{tenantId}/{path}".Replace("//", "/");
            try
            {
                var attachment = await attachmentService.GetAsync(path);
                return File(attachment.FileContent, $"application/{attachment.Ext}", attachment.Path.Split('/').Last().Replace($".{attachment.Ext}", ""));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}