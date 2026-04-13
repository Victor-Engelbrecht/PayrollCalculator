using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public interface IPayrollRule
{
    void Apply(PayCalculationContext context, IList<RuleViolation> violations);
}
