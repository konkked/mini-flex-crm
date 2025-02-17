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
    public async Task<IActionResult> ListCustomers([FromQuery] int pageSize = 50,
        [FromQuery] string? next = null,
        [FromQuery] string? prev = null,
        [FromQuery] string? search = null) =>
        string.IsNullOrEmpty(prev)
            ? Ok(await customerService.ListItems(pageSize, next, search))
            : Ok(await customerService.ListPreviousItems(pageSize, prev, search));

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