using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Rules;

public class PayrollRuleFactory
{
    public IReadOnlyList<IPayrollRule> GetRules(Employee employee)
    {
        // TODO: load rule config per employee/company from DB
        return [new BaseSalaryRule(), new FlatTaxRule(0.20m), new MinimumWageRule(1500.00m)];
    }
}
