using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class BaseSalaryRule(decimal salary) : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Addition;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        context.TotalAdditions += salary;
        context.LineItems.Add(new PayslipLineItem
        {
            RuleName    = nameof(BaseSalaryRule),
            Description = "Base salary",
            Amount      = salary,
            Kind        = PayslipLineItemKind.Addition
        });
    }
}
