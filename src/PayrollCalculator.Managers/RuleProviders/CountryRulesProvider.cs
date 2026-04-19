using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class CountryRulesProvider(
    ICountryPayrollConfigRepository _configRepo) : IPayrollRuleProvider
{
    public async Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        if (context.Employee.CountryCode is null)
            return [];

        var config = await _configRepo.GetAsync(context.Employee.CountryCode);
        if (config is null)
            return [];

        return [new MinimumWageRule(config.MinimumWage)];
    }
}
