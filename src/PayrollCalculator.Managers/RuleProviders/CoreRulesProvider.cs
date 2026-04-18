using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class CoreRulesProvider : IPayrollRuleProvider
{
    public Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
        => Task.FromResult<IEnumerable<IPayrollRule>>([new BaseSalaryRule()]);
}
