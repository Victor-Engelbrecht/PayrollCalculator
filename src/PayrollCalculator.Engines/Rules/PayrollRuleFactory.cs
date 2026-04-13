using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class PayrollRuleFactory
{
    public IReadOnlyList<IPayrollRule> GetRules(Employee employee)
    {
        return [new FlatTaxRule(), new MinimumWageRule()];
    }
}
