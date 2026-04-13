using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class FlatTaxRule : IPayrollRule
{
    private const decimal TaxRate = 0.20m;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        var tax = Math.Round(context.GrossPay * TaxRate, 2);
        context.NetPay -= tax;
    }
}
