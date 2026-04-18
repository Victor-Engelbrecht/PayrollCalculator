namespace PayrollCalculator.Domain.Models;

public record EmployeePayslip
{
    public Employee Employee { get; init; } = null!;
    public PayslipDetail Payslip { get; init; } = null!;
}
