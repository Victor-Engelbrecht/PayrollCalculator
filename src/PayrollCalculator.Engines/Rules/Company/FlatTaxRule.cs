using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class FlatTaxRule(decimal taxRate) : IPayrollRule
{
    public PayrollRuleEffect Effect => PayrollRuleEffect.Deduction;

    public void Apply(PayCalculationContext context, IList<RuleViolation> violations)
    {
        var amount = Math.Round(context.GrossPay * taxRate, 2);
        context.TotalDeductions += amount;
        context.LineItems.Add(new PayslipLineItem
        {
            RuleName    = nameof(FlatTaxRule),
            Description = $"Flat tax deduction ({taxRate:P0})",
            Amount      = amount,
            Kind        = PayslipLineItemKind.Deduction
        });
    }
}
