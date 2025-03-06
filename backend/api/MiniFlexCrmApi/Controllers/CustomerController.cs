using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/customer")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet("{id}/relationships")]
    public async Task<ActionResult<CustomerModel>> GetCustomerWithRelationships([FromRoute] int tenantId, [FromRoute] int id)
        => Ok(await customerService.GetCustomerWithRelationship(tenantId, id));

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerModel>> GetCustomer(
        [FromRoute] int tenantId, [FromRoute] int id) =>
        Ok(await customerService.GetItemAsync(tenantId, id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerModel>>> ListCustomers(
        [FromRoute] int tenantId,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0,
        [FromQuery] string? search = null)
        => Ok(await customerService.ListItemsAsync(limit, offset, search,
            parameters: new Dictionary<string, object> { ["tenant_id"] = tenantId }));

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId,  [FromBody] CustomerModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(context.TenantId != 0)
            return Forbid();
        return await customerService.CreateItemAsync(model) ? Ok() : BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateCustomer([FromRequestContext] RequestContext context, 
        [FromRoute] int tenantId, 
        [FromRoute] int id, [FromBody] CustomerModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(context.TenantId != 0)
            return Forbid();
        return await customerService.UpdateItemAsync(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteCustomer([FromRoute] int tenantId, [FromRoute] int id) =>
        await customerService.DeleteItemAsync(tenantId, id) ? Ok() : NotFound();
}