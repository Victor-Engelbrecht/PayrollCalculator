using PayrollCalculator.Domain.Models;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class CountryPayrollConfigRepository : ICountryPayrollConfigRepository
{
    public Task<CountryPayrollConfig?> GetAsync(string countryCode)
        => throw new NotImplementedException();
}
