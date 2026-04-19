using PayrollCalculator.Domain.Models;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class CompanyPayrollConfigRepository : ICompanyPayrollConfigRepository
{
    public Task<CompanyPayrollConfig?> GetAsync(int companyId)
        => throw new NotImplementedException();
}
