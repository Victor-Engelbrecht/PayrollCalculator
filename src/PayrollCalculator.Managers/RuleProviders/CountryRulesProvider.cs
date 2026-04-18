using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class CountryRulesProvider : IPayrollRuleProvider
{
    public Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        // TODO: load statutory compliance rules from DB by context.Employee.CountryCode
        if (context.Employee.CountryCode is null)
            return Task.FromResult(Enumerable.Empty<IPayrollRule>());

        return Task.FromResult<IEnumerable<IPayrollRule>>([new MinimumWageRule(1500.00m)]);
    }
}
