using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Engines;

public class PayCalculationEngine : IPayCalculationEngine
{
    public PayCalculationResult Calculate(
        Employee employee,
        IEnumerable<IPayrollRule> rules)
    {
        var context    = new PayCalculationContext { Employee = employee };
        var violations = new List<RuleViolation>();
        var ruleList   = rules.ToList();

        foreach (var rule in ruleList.Where(r => r.Effect == PayrollRuleEffect.Addition))
            rule.Apply(context, violations);

        foreach (var rule in ruleList.Where(r => r.Effect == PayrollRuleEffect.Deduction))
            rule.Apply(context, violations);

        foreach (var rule in ruleList.Where(r => r.Effect == PayrollRuleEffect.Compliance))
            rule.Apply(context, violations);

        return new PayCalculationResult
        {
            NetAmount       = context.NetPay,
            TotalAdditions  = context.TotalAdditions,
            TotalDeductions = context.TotalDeductions,
            Violations      = violations,
            LineItems       = context.LineItems.AsReadOnly()
        };
    }
}
