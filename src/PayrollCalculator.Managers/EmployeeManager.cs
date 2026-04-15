using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.Managers;

public class EmployeeManager : IEmployeeManager
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayCalculationEngine _payCalculationEngine;
    private readonly PayrollRuleFactory _ruleFactory;
    private readonly IWideEventContext _wideEvent;

    public EmployeeManager(
        IEmployeeRepository employeeRepository,
        IPayCalculationEngine payCalculationEngine,
        PayrollRuleFactory ruleFactory,
        IWideEventContext wideEvent)
    {
        _employeeRepository = employeeRepository;
        _payCalculationEngine = payCalculationEngine;
        _ruleFactory = ruleFactory;
        _wideEvent = wideEvent;
    }

    public Task<Employee?> GetEmployeeAsync(int id)
    {
        _wideEvent.Set("operation", "employee.get");
        _wideEvent.Set("employee_id", id);
        return _employeeRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId)
    {
        _wideEvent.Set("operation", "employee.list_by_company");
        _wideEvent.Set("company_id", companyId);
        return _employeeRepository.GetByCompanyIdAsync(companyId);
    }

    public async Task<int> CreateEmployeeAsync(Employee employee)
    {
        _wideEvent.Set("operation", "employee.create");
        _wideEvent.Set("company_id", employee.CompanyId);
        var id = await _employeeRepository.CreateAsync(employee);
        _wideEvent.Set("employee_id", id);
        return id;
    }

    public Task UpdateEmployeeAsync(Employee employee)
    {
        _wideEvent.Set("operation", "employee.update");
        _wideEvent.Set("employee_id", employee.Id);
        _wideEvent.Set("company_id", employee.CompanyId);
        return _employeeRepository.UpdateAsync(employee);
    }

    public Task DeleteEmployeeAsync(int id)
    {
        _wideEvent.Set("operation", "employee.delete");
        _wideEvent.Set("employee_id", id);
        return _employeeRepository.DeleteAsync(id);
    }

    public async Task<PayCalculationResult> CalculatePayAsync(int employeeId)
    {
        _wideEvent.Set("operation", "employee.calculate_pay");
        _wideEvent.Set("employee_id", employeeId);

        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new InvalidOperationException($"Employee {employeeId} not found.");

        _wideEvent.Set("company_id", employee.CompanyId);

        var rules = _ruleFactory.GetRules(employee);
        var result = _payCalculationEngine.Calculate(employee, rules);

        _wideEvent.Set("net_amount", result.NetAmount);
        _wideEvent.Set("total_additions", result.TotalAdditions);
        _wideEvent.Set("total_deductions", result.TotalDeductions);
        _wideEvent.Set("rule_violations_count", result.Violations.Count);

        return result;
    }
}
