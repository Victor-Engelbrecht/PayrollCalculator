
using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class MinimumWageRule : IPayrollRule
{
    private const decimal MinimumMonthlyNetPay = 1500.00m;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        if (context.NetPay < MinimumMonthlyNetPay)
            violations.Add(new RuleViolation
            {
                RuleName = nameof(MinimumWageRule),
                Message = $"Net pay {context.NetPay:C} is below the minimum of {MinimumMonthlyNetPay:C}."
            });
    }
}
