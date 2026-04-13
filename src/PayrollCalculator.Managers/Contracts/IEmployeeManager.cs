using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Managers.Contracts;

public interface IEmployeeManager
{
    Task<Employee?> GetEmployeeAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId);
    Task<int> CreateEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int id);

    Task<PayCalculationResult> CalculatePayAsync(int employeeId);
}
