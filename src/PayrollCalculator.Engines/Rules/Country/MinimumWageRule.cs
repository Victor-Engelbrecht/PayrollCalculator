using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class MinimumWageRule(decimal minimumNetPay) : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Compliance;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        if (context.NetPay < minimumNetPay)
            violations.Add(new RuleViolation
            {
                RuleName = nameof(MinimumWageRule),
                Message  = $"Net pay {context.NetPay:C} is below the minimum of {minimumNetPay:C}."
            });
    }
}
