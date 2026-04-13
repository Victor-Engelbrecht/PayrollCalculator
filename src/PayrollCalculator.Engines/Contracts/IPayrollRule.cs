using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public interface IPayrollRule
{
    PayrollRuleEffect Effect { get; }
    void Apply(PayCalculationContext context, IList<RuleViolation> violations);
}
