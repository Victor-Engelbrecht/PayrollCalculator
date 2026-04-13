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

}
