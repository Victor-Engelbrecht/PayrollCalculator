using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class PayCalculationContextBuilder
{
    private decimal _totalAdditions  = 0m;
    private decimal _totalDeductions = 0m;

    public PayCalculationContextBuilder WithTotalAdditions(decimal additions)   { _totalAdditions  = additions;  return this; }
    public PayCalculationContextBuilder WithTotalDeductions(decimal deductions) { _totalDeductions = deductions; return this; }
    public PayCalculationContextBuilder WithGrossPay(decimal grossPay)          { _totalAdditions  = grossPay;   return this; }
    public PayCalculationContextBuilder WithNetPay(decimal netPay)              { _totalAdditions  = netPay;     return this; }

    public PayCalculationContext Build() => new()
    {
        TotalAdditions  = _totalAdditions,
        TotalDeductions = _totalDeductions
    };
}
