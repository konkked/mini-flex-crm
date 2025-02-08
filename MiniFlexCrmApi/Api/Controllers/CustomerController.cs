using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Models.Public;

[ApiController]
[Route("api/tenant/{tenantId}/customer")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id) =>
        Ok(await customerService.GetItem(id));

    [HttpGet]
    public async Task<IActionResult> ListCustomers([FromQuery] int pageSize = 50, [FromQuery] string? next = null) =>
        Ok(await customerService.ListItems(pageSize, next));

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerModel model) =>
        await customerService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerModel model)
    {
        model.Id = id;
        return await customerService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id) =>
        await customerService.DeleteItem(id) ? Ok() : NotFound();
}