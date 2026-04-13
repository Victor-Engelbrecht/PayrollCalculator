using Microsoft.AspNetCore.Mvc;
using PayrollCalculator.Client.Requests;
using PayrollCalculator.Client.Responses;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Managers.Contracts;

namespace PayrollCalculator.Client.Controllers;

[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeManager _employeeManager;

    public EmployeesController(IEmployeeManager employeeManager)
    {
        _employeeManager = employeeManager;
    }

    [HttpGet("employees/{id}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployee(int id)
    {
        var employee = await _employeeManager.GetEmployeeAsync(id);
        if (employee is null) return NotFound();
        return Ok(ToResponse(employee));
    }

    [HttpGet("companies/{companyId}/employees")]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetEmployeesByCompany(int companyId)
    {
        var employees = await _employeeManager.GetEmployeesByCompanyAsync(companyId);
        return Ok(employees.Select(ToResponse));
    }

    [HttpPost("employees")]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(CreateEmployeeRequest request)
    {
        var employee = new Employee
        {
            CompanyId = request.CompanyId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            BaseSalary = request.BaseSalary,
            BankAccountNumber = request.BankAccountNumber
        };

        var id = await _employeeManager.CreateEmployeeAsync(employee);
        var created = await _employeeManager.GetEmployeeAsync(id);
        return CreatedAtAction(nameof(GetEmployee), new { id }, ToResponse(created!));
    }

    [HttpPut("employees/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequest request)
    {
        var existing = await _employeeManager.GetEmployeeAsync(id);
        if (existing is null) return NotFound();

        var updated = existing with
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            BaseSalary = request.BaseSalary,
            BankAccountNumber = request.BankAccountNumber
        };

        await _employeeManager.UpdateEmployeeAsync(updated);
        return NoContent();
    }

    [HttpDelete("employees/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        await _employeeManager.DeleteEmployeeAsync(id);
        return NoContent();
    }

    private static EmployeeResponse ToResponse(Employee e) =>
        new(e.Id, e.CompanyId, e.FirstName, e.LastName, e.Email, e.BaseSalary, e.BankAccountNumber, e.CreatedAt, e.UpdatedAt);
}
