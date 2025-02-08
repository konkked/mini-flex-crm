using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Models.Public;

[ApiController]
[Route("api/tenant/{tenantId}/user")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id) =>
        Ok(await userService.GetItem(id));

    [HttpGet]
    public async Task<IActionResult> ListUsers([FromQuery] int pageSize = 50, [FromQuery] string? next = null) =>
        Ok(await userService.ListItems(pageSize, next));

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserModel model) =>
        await userService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserModel model)
    {
        model.Id = id;
        return await userService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id) =>
        await userService.DeleteItem(id) ? Ok() : NotFound();
}