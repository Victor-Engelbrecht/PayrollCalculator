using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class PayslipDetailBuilder
{
    private int     _id              = 1;
    private int     _payrollId       = 1;
    private int     _employeeId      = 1;
    private decimal _baseSalary      = 5000m;
    private decimal _totalAdditions  = 5000m;
    private decimal _totalDeductions = 1000m;
    private decimal _netAmount       = 4000m;

    public PayslipDetailBuilder WithId(int id)                          { _id              = id;              return this; }
    public PayslipDetailBuilder WithPayrollId(int payrollId)            { _payrollId       = payrollId;       return this; }
    public PayslipDetailBuilder WithEmployeeId(int employeeId)          { _employeeId      = employeeId;      return this; }
    public PayslipDetailBuilder WithBaseSalary(decimal baseSalary)      { _baseSalary      = baseSalary;      return this; }
    public PayslipDetailBuilder WithTotalAdditions(decimal additions)   { _totalAdditions  = additions;       return this; }
    public PayslipDetailBuilder WithTotalDeductions(decimal deductions) { _totalDeductions = deductions;      return this; }
    public PayslipDetailBuilder WithNetAmount(decimal netAmount)        { _netAmount       = netAmount;       return this; }

    public PayslipDetail Build() => new()
    {
        Id              = _id,
        PayrollId       = _payrollId,
        EmployeeId      = _employeeId,
        BaseSalary      = _baseSalary,
        TotalAdditions  = _totalAdditions,
        TotalDeductions = _totalDeductions,
        NetAmount       = _netAmount
    };
}
