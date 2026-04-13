using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories.Contracts;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(int id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task<int> CreateAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(int id);
}
