using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.Engines;

public class PayCalculationEngine : IPayCalculationEngine
{
    public PayCalculationResult Calculate(
        Employee employee,
        IEnumerable<Addition> additions,
        IEnumerable<Deduction> deductions,
        IEnumerable<IPayrollRule> rules)
    {
        var additionList = additions.ToList();
        var deductionList = deductions.ToList();

        var totalAdditions = additionList.Sum(a => a.Amount);
        var totalDeductions = deductionList.Sum(d => d.Amount);
        var grossPay = employee.BaseSalary + totalAdditions;
        var netPay = grossPay - totalDeductions;

        var context = new PayCalculationContext
        {
            Employee = employee,
            Additions = additionList,
            Deductions = deductionList,
            GrossPay = grossPay,
            TotalDeductions = totalDeductions,
            NetPay = netPay
        };

        var violations = new List<RuleViolation>();

        foreach (var rule in rules)
            rule.Apply(context, violations);

        return new PayCalculationResult
        {
            NetAmount = context.NetPay,
            TotalAdditions = totalAdditions,
            TotalDeductions = totalDeductions,
            Violations = violations
        };
    }
}
