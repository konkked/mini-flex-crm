using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniFlexCrmApi.Api.Models;
using MiniFlexCrmApi.Api.Services;

namespace MiniFlexCrmApi.Api.Controllers;

[ApiController]
[Route("api/tenant/{tenantId}/company")]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCompany(int id) =>
        Ok(await companyService.GetItem(id));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyModel>>> ListCompanies([FromQuery] int limit = 50, [FromQuery] int offset = 0) 
        => Ok(await companyService.ListItems(limit, offset));

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyModel model) =>
        await companyService.CreateItem(model) ? Ok() : BadRequest();

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, [FromBody] CompanyModel model)
    {
        model.Id = id;
        return await companyService.UpdateItem(model) ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id) =>
        await companyService.DeleteItem(id) ? Ok() : NotFound();
}