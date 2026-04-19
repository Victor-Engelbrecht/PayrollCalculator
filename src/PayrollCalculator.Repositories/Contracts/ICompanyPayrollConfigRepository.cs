using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories.Contracts;

public interface ICompanyPayrollConfigRepository
{
    Task<CompanyPayrollConfig?> GetAsync(int companyId);
}
