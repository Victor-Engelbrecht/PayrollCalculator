using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Engines.Contracts;

public interface IPayCalculationEngine
{
    PayCalculationResult Calculate(
        Employee employee,
        IEnumerable<IPayrollRule> rules);
}
