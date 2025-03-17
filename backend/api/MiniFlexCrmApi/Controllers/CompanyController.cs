using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;
using MiniFlexCrmApi.Services;

namespace MiniFlexCrmApi.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/company")]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany([FromRoute] int tenantId, 
        [FromRoute] int id) =>
        Ok(await companyService.GetAsync(id, tenantId));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyModel>>> ListCompanies([FromRoute] int tenantId, [FromQuery] int limit = 50, [FromQuery] int offset = 0) 
        => Ok(await companyService.ListAsync(limit, offset, 
            query: null, parameters: new Dictionary<string, object> { ["tenant_id"] = tenantId }));

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromRequestContext] RequestContext requestContext, 
        [FromRoute] int tenantId, [FromBody] CompanyModel model) {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(requestContext.TenantId != 0)
                return Forbid();
        await companyService.CreateAsync(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany([FromRequestContext] RequestContext requestContext,
        [FromRoute] int tenantId, [FromRoute] int id, [FromBody] CompanyModel model)
    {
        if (tenantId != 0)
        {
            if (model.TenantId != tenantId)
                return Forbid();
            model.TenantId = tenantId;
        }
        else if(requestContext.TenantId != 0)
            return Forbid();
        model.Id = id;
        model.TenantId = tenantId;
        return await companyService.UpdateAsync(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id) =>
        await companyService.DeleteAsync(id) ? Ok() : NotFound();
}