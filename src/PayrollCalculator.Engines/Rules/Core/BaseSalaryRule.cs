using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class BaseSalaryRule : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Addition;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        context.TotalAdditions += context.Employee.BaseSalary;
        context.LineItems.Add(new PayslipLineItem
        {
            RuleName    = nameof(BaseSalaryRule),
            Description = "Base salary",
            Amount      = context.Employee.BaseSalary,
            Kind        = PayslipLineItemKind.Addition
        });
    }
}
