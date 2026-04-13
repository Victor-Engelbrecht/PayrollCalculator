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

    [HttpGet("employees/{id}/additions")]
    public async Task<ActionResult<IEnumerable<AdditionResponse>>> GetAdditions(int id)
    {
        var additions = await _employeeManager.GetAdditionsAsync(id);
        return Ok(additions.Select(a => new AdditionResponse(a.Id, a.EmployeeId, a.PayrollId, a.Description, a.Amount)));
    }

    [HttpPost("employees/{id}/additions")]
    public async Task<ActionResult<AdditionResponse>> AddAddition(int id, AddAdditionRequest request)
    {
        var addition = new Addition
        {
            EmployeeId = id,
            Description = request.Description,
            Amount = request.Amount
        };

        var additionId = await _employeeManager.AddAdditionAsync(addition);
        var additions = await _employeeManager.GetAdditionsAsync(id);
        var created = additions.First(a => a.Id == additionId);
        return CreatedAtAction(nameof(GetAdditions), new { id }, new AdditionResponse(created.Id, created.EmployeeId, created.PayrollId, created.Description, created.Amount));
    }

    [HttpDelete("employees/{employeeId}/additions/{additionId}")]
    public async Task<IActionResult> RemoveAddition(int employeeId, int additionId)
    {
        await _employeeManager.RemoveAdditionAsync(additionId);
        return NoContent();
    }

    [HttpGet("employees/{id}/deductions")]
    public async Task<ActionResult<IEnumerable<DeductionResponse>>> GetDeductions(int id)
    {
        var deductions = await _employeeManager.GetDeductionsAsync(id);
        return Ok(deductions.Select(d => new DeductionResponse(d.Id, d.EmployeeId, d.PayrollId, d.Description, d.Amount)));
    }

    [HttpPost("employees/{id}/deductions")]
    public async Task<ActionResult<DeductionResponse>> AddDeduction(int id, AddDeductionRequest request)
    {
        var deduction = new Deduction
        {
            EmployeeId = id,
            Description = request.Description,
            Amount = request.Amount
        };

        var deductionId = await _employeeManager.AddDeductionAsync(deduction);
        var deductions = await _employeeManager.GetDeductionsAsync(id);
        var created = deductions.First(d => d.Id == deductionId);
        return CreatedAtAction(nameof(GetDeductions), new { id }, new DeductionResponse(created.Id, created.EmployeeId, created.PayrollId, created.Description, created.Amount));
    }

    [HttpDelete("employees/{employeeId}/deductions/{deductionId}")]
    public async Task<IActionResult> RemoveDeduction(int employeeId, int deductionId)
    {
        await _employeeManager.RemoveDeductionAsync(deductionId);
        return NoContent();
    }

    private static EmployeeResponse ToResponse(Employee e) =>
        new(e.Id, e.CompanyId, e.FirstName, e.LastName, e.Email, e.BaseSalary, e.BankAccountNumber, e.CreatedAt, e.UpdatedAt);
}
