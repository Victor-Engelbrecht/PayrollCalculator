using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Engines.Contracts;

public interface IPayrollRuleProvider
{
    Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context);
}
