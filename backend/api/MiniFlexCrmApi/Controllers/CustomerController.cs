using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/customer")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("{id}/relationships")]
    public async Task<IActionResult> GetCustomerWithRelationships(int id)
        => Ok(await customerService.GetCustomerWithRelationship(id));
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id) =>
        Ok(await customerService.GetItem(id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerModel>>> ListCustomers(
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null)
            => Ok(await customerService.ListItems(limit, offset, search));

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