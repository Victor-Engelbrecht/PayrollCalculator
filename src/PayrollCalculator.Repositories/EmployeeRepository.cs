using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId) => throw new NotImplementedException();
    public Task<int> CreateAsync(Employee employee) => throw new NotImplementedException();
    public Task UpdateAsync(Employee employee) => throw new NotImplementedException();
    public Task DeleteAsync(int id) => throw new NotImplementedException();

    public Task<IEnumerable<Addition>> GetAdditionsByEmployeeIdAsync(int employeeId) => throw new NotImplementedException();
    public Task<int> CreateAdditionAsync(Addition addition) => throw new NotImplementedException();
    public Task DeleteAdditionAsync(int id) => throw new NotImplementedException();

    public Task<IEnumerable<Deduction>> GetDeductionsByEmployeeIdAsync(int employeeId) => throw new NotImplementedException();
    public Task<int> CreateDeductionAsync(Deduction deduction) => throw new NotImplementedException();
    public Task DeleteDeductionAsync(int id) => throw new NotImplementedException();
}
