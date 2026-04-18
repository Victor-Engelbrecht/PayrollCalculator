using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Engines.Contracts;

public interface IPayrollRuleFactory
{
    Task<IReadOnlyList<IPayrollRule>> GetRulesAsync(Company company, Employee employee);
}
