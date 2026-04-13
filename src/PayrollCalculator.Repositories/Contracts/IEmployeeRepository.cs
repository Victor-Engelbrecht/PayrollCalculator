using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories.Contracts;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId);
    Task<int> CreateAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(int id);

    Task<IEnumerable<Addition>> GetAdditionsByEmployeeIdAsync(int employeeId);
    Task<int> CreateAdditionAsync(Addition addition);
    Task DeleteAdditionAsync(int id);

    Task<IEnumerable<Deduction>> GetDeductionsByEmployeeIdAsync(int employeeId);
    Task<int> CreateDeductionAsync(Deduction deduction);
    Task DeleteDeductionAsync(int id);
}
