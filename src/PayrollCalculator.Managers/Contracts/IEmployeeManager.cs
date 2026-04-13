using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Managers.Contracts;

public interface IEmployeeManager
{
    Task<Employee?> GetEmployeeAsync(int id);
    Task<IEnumerable<Employee>> GetEmployeesByCompanyAsync(int companyId);
    Task<int> CreateEmployeeAsync(Employee employee);
    Task UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int id);

    Task<IEnumerable<Addition>> GetAdditionsAsync(int employeeId);
    Task<int> AddAdditionAsync(Addition addition);
    Task RemoveAdditionAsync(int id);

    Task<IEnumerable<Deduction>> GetDeductionsAsync(int employeeId);
    Task<int> AddDeductionAsync(Deduction deduction);
    Task RemoveDeductionAsync(int id);

    Task<PayCalculationResult> CalculatePayAsync(int employeeId);
}
