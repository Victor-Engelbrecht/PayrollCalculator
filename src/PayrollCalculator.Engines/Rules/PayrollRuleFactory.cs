using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;

namespace PayrollCalculator.Engines.Rules;

public sealed class PayrollRuleFactory(IEnumerable<IPayrollRuleProvider> providers) : IPayrollRuleFactory
{
    public async Task<IReadOnlyList<IPayrollRule>> GetRulesAsync(Company company, Employee employee)
    {
        var context = new PayrollRuleContext { Company = company, Employee = employee };
        var rules = new List<IPayrollRule>();
        foreach (var provider in providers)
            rules.AddRange(await provider.GetRulesAsync(context));
        return rules.AsReadOnly();
    }
}
