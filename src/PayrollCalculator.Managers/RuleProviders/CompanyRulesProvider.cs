using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class CompanyRulesProvider : IPayrollRuleProvider
{
    public Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        // TODO: load company tax rules from DB by context.Company.Id
        return Task.FromResult<IEnumerable<IPayrollRule>>([new FlatTaxRule(0.20m)]);
    }
}
