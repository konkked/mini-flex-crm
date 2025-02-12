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
    public async Task<IActionResult> ListCompanies([FromQuery] int pageSize = 50, [FromQuery] string? next = null, [FromQuery] string? search = null) =>
        Ok(await companyService.ListItems(pageSize, next));
    
    [HttpGet]
    public async Task<IActionResult> ListPreviousCompanies([FromQuery] int pageSize = 50, [FromQuery] string? prev = null) =>
        Ok(await companyService.ListPreviousItems(pageSize, prev));

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