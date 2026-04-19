using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class PayCalculationResultBuilder
{
    private decimal                      _netAmount       = 4000m;
    private decimal                      _totalAdditions  = 5000m;
    private decimal                      _totalDeductions = 1000m;
    private IReadOnlyList<RuleViolation> _violations      = [];
    private IReadOnlyList<PayslipLineItem> _lineItems     = [];

    public PayCalculationResultBuilder WithNetAmount(decimal amount)                       { _netAmount       = amount;     return this; }
    public PayCalculationResultBuilder WithTotalAdditions(decimal additions)               { _totalAdditions  = additions;  return this; }
    public PayCalculationResultBuilder WithTotalDeductions(decimal deductions)             { _totalDeductions = deductions; return this; }
    public PayCalculationResultBuilder WithViolations(IReadOnlyList<RuleViolation> v)      { _violations      = v;          return this; }
    public PayCalculationResultBuilder WithLineItems(IReadOnlyList<PayslipLineItem> items) { _lineItems       = items;      return this; }

    public PayCalculationResult Build() => new()
    {
        NetAmount       = _netAmount,
        TotalAdditions  = _totalAdditions,
        TotalDeductions = _totalDeductions,
        Violations      = _violations,
        LineItems       = _lineItems
    };
}
