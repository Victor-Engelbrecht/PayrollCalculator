using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Managers.RuleProviders;

public sealed class EmployeeRulesProvider : IPayrollRuleProvider
{
    public Task<IEnumerable<IPayrollRule>> GetRulesAsync(PayrollRuleContext context)
    {
        // TODO: load employee-specific rules from DB by context.Employee.Id
        return Task.FromResult(Enumerable.Empty<IPayrollRule>());
    }
}
