using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Managers.Contracts;

public interface ICompanyManager
{
    Task<Company?> GetCompanyAsync(int id);
    Task<IEnumerable<Company>> GetAllCompaniesAsync();
    Task<int> CreateCompanyAsync(Company company);
    Task UpdateCompanyAsync(Company company);
    Task DeleteCompanyAsync(int id);
}
