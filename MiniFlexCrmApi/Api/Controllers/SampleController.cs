using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Middleware;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Models.Public;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpPost("test/{id}")]
    public IActionResult Test(
        [FromRequestContext] RequestContext requestContext,    // Auto-injected cloned context
        [FromRoute] int id,               // Path variable
        [FromQuery] string filter,        // Query parameter
        [FromBody] UserModel userModel)   // JSON request body
    {
        return Ok(new
        {
            requestContext,
            id,
            filter,
            userModel,
        });
    }
}