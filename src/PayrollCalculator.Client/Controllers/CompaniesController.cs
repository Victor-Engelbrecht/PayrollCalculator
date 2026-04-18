using Microsoft.AspNetCore.Mvc;
using PayrollCalculator.Client.Requests;
using PayrollCalculator.Client.Responses;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Managers.Contracts;

namespace PayrollCalculator.Client.Controllers;

[ApiController]
[Route("companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyManager _companyManager;
    private readonly IPayrollManager _payrollManager;

    public CompaniesController(ICompanyManager companyManager, IPayrollManager payrollManager)
    {
        _companyManager = companyManager;
        _payrollManager = payrollManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyResponse>>> GetAllCompanies()
    {
        var companies = await _companyManager.GetAllCompaniesAsync();
        return Ok(companies.Select(ToResponse));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyResponse>> GetCompany(int id)
    {
        var company = await _companyManager.GetCompanyAsync(id);
        if (company is null) return NotFound();
        return Ok(ToResponse(company));
    }

    [HttpPost]
    public async Task<ActionResult<CompanyResponse>> CreateCompany(CreateCompanyRequest request)
    {
        var company = new Company
        {
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            ContactEmail = request.ContactEmail
        };

        var id = await _companyManager.CreateCompanyAsync(company);
        var created = await _companyManager.GetCompanyAsync(id);
        return CreatedAtAction(nameof(GetCompany), new { id }, ToResponse(created!));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, UpdateCompanyRequest request)
    {
        var existing = await _companyManager.GetCompanyAsync(id);
        if (existing is null) return NotFound();

        var updated = existing with
        {
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            ContactEmail = request.ContactEmail
        };

        await _companyManager.UpdateCompanyAsync(updated);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        await _companyManager.DeleteCompanyAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/payroll/run")]
    public async Task<ActionResult<PayrollSummaryResponse>> RunPayroll(int id)
    {
        var company = await _companyManager.GetCompanyAsync(id);
        if (company is null) return NotFound();

        var summary = await _payrollManager.RunPayrollAsync(id);

        return Ok(new PayrollSummaryResponse(
            summary.PayrollId,
            summary.EmployeeCount,
            summary.TotalNetPaid,
            summary.Violations.Select(v => new RuleViolationResponse(v.RuleName, v.Message)).ToList(),
            summary.CompletedAt));
    }

    private static CompanyResponse ToResponse(Company c) =>
        new(c.Id, c.Name, c.RegistrationNumber, c.ContactEmail, c.CreatedAt, c.UpdatedAt);
}
