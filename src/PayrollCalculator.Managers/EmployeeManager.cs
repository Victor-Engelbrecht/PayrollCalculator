using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Managers;

public class EmployeeManager : IEmployeeManager
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayCalculationEngine _payCalculationEngine;
    private readonly PayrollRuleFactory _ruleFactory;

    public EmployeeManager(
        IEmployeeRepository employeeRepository,
        IPayCalculationEngine payCalculationEngine,
        PayrollRuleFactory ruleFactory)
    {
        _employeeRepository = employeeRepository;
        _payCalculationEngine = payCalculationEngine;
        _ruleFactory = ruleFactory;
    }

    public Task<Employee?> GetEmployeeAsync(int id) =>
        _employeeRepository.GetByIdAsync(id);

    public Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId) =>
        _employeeRepository.GetByCompanyIdAsync(companyId);

    public Task<int> CreateEmployeeAsync(Employee employee) =>
        _employeeRepository.CreateAsync(employee);

    public Task UpdateEmployeeAsync(Employee employee) =>
        _employeeRepository.UpdateAsync(employee);

    public Task DeleteEmployeeAsync(int id) =>
        _employeeRepository.DeleteAsync(id);

    public Task<IEnumerable<Addition>> GetAdditionsAsync(int employeeId) =>
        _employeeRepository.GetAdditionsByEmployeeIdAsync(employeeId);

    public Task<int> AddAdditionAsync(Addition addition) =>
        _employeeRepository.CreateAdditionAsync(addition);

    public Task RemoveAdditionAsync(int id) =>
        _employeeRepository.DeleteAdditionAsync(id);

    public Task<IEnumerable<Deduction>> GetDeductionsAsync(int employeeId) =>
        _employeeRepository.GetDeductionsByEmployeeIdAsync(employeeId);

    public Task<int> AddDeductionAsync(Deduction deduction) =>
        _employeeRepository.CreateDeductionAsync(deduction);

    public Task RemoveDeductionAsync(int id) =>
        _employeeRepository.DeleteDeductionAsync(id);

    public async Task<PayCalculationResult> CalculatePayAsync(int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId)
            ?? throw new InvalidOperationException($"Employee {employeeId} not found.");

        var additions = await _employeeRepository.GetAdditionsByEmployeeIdAsync(employeeId);
        var deductions = await _employeeRepository.GetDeductionsByEmployeeIdAsync(employeeId);
        var rules = _ruleFactory.GetRules(employee);

        return _payCalculationEngine.Calculate(employee, additions, deductions, rules);
    }
}
