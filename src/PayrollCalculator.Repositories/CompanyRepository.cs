using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories;

public class CompanyRepository : ICompanyRepository
{
    public Task<Company?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Company>> GetAllAsync() => throw new NotImplementedException();
    public Task<int> CreateAsync(Company company) => throw new NotImplementedException();
    public Task UpdateAsync(Company company) => throw new NotImplementedException();
    public Task DeleteAsync(int id) => throw new NotImplementedException();
}
