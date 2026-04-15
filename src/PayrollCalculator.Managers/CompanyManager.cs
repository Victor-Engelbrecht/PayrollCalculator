using PayrollCalculator.Domain.Models;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.Managers;

public class CompanyManager : ICompanyManager
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IWideEventContext _wideEvent;

    public CompanyManager(ICompanyRepository companyRepository, IWideEventContext wideEvent)
    {
        _companyRepository = companyRepository;
        _wideEvent = wideEvent;
    }

    public Task<Company?> GetCompanyAsync(int id)
    {
        _wideEvent.Set("operation", "company.get");
        _wideEvent.Set("company_id", id);
        return _companyRepository.GetByIdAsync(id);
    }

    public Task<IEnumerable<Company>> GetAllCompaniesAsync()
    {
        _wideEvent.Set("operation", "company.list");
        return _companyRepository.GetAllAsync();
    }

    public async Task<int> CreateCompanyAsync(Company company)
    {
        _wideEvent.Set("operation", "company.create");
        _wideEvent.Set("company_registration_number", company.RegistrationNumber);
        var id = await _companyRepository.CreateAsync(company);
        _wideEvent.Set("company_id", id);
        return id;
    }

    public Task UpdateCompanyAsync(Company company)
    {
        _wideEvent.Set("operation", "company.update");
        _wideEvent.Set("company_id", company.Id);
        return _companyRepository.UpdateAsync(company);
    }

    public Task DeleteCompanyAsync(int id)
    {
        _wideEvent.Set("operation", "company.delete");
        _wideEvent.Set("company_id", id);
        return _companyRepository.DeleteAsync(id);
    }
}
