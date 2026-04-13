using Microsoft.AspNetCore.Mvc;
using PayrollCalculator.Client.Requests;
using PayrollCalculator.Client.Responses;
using PayrollCalculator.Managers.Contracts;

namespace PayrollCalculator.Client.Controllers;

[ApiController]
[Route("payroll")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollManager _payrollManager;

    public PayrollController(IPayrollManager payrollManager)
    {
        _payrollManager = payrollManager;
    }

    [HttpPost("run")]
    public async Task<ActionResult<PayrollSummaryResponse>> RunPayroll(RunPayrollRequest request)
    {
        var summary = await _payrollManager.RunPayrollAsync(request.CompanyId, request.PeriodStart, request.PeriodEnd);

        var response = new PayrollSummaryResponse(
            summary.PayrollId,
            summary.EmployeeCount,
            summary.TotalNetPaid,
            summary.Violations.Select(v => new RuleViolationResponse(v.RuleName, v.Message)).ToList(),
            summary.CompletedAt);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PayrollResponse>> GetPayroll(int id)
    {
        var payroll = await _payrollManager.GetPayrollAsync(id);
        if (payroll is null) return NotFound();

        return Ok(new PayrollResponse(
            payroll.Id,
            payroll.CompanyId,
            payroll.PeriodStart,
            payroll.PeriodEnd,
            payroll.RunAt,
            payroll.Status));
    }

    [HttpGet("{id}/payslips")]
    public async Task<ActionResult<IEnumerable<PayslipResponse>>> GetPayslips(int id)
    {
        var payslips = await _payrollManager.GetPayslipsAsync(id);

        return Ok(payslips.Select(p => new PayslipResponse(
            p.Id,
            p.PayrollId,
            p.EmployeeId,
            p.BaseSalary,
            p.TotalAdditions,
            p.TotalDeductions,
            p.NetAmount,
            p.PaymentReference)));
    }
}
