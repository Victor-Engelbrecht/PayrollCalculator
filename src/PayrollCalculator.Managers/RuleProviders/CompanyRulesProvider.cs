using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class CompanyRulesProvider(
    ICompanyPayrollConfigRepository _configRepo) : IPayrollRuleProvider
{
    public async Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        var config = await _configRepo.GetAsync(context.Company.Id);
        if (config is null)
            return [];

        return [new FlatTaxRule(config.TaxRate)];
    }
}
