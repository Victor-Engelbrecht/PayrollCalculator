using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class FlatTaxRule(decimal taxRate) : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Deduction;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        context.TotalDeductions += Math.Round(context.GrossPay * taxRate, 2);
    }
}
