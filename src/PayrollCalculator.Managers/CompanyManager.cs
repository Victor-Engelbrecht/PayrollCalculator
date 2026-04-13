using PayrollCalculator.Domain.Models;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Managers;

public class CompanyManager : ICompanyManager
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyManager(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public Task<Company?> GetCompanyAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Company>> GetAllCompaniesAsync() => throw new NotImplementedException();
    public Task<int> CreateCompanyAsync(Company company) => throw new NotImplementedException();
    public Task UpdateCompanyAsync(Company company) => throw new NotImplementedException();
    public Task DeleteCompanyAsync(int id) => throw new NotImplementedException();
}
