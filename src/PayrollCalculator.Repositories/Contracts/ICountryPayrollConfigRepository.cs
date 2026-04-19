using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories.Contracts;

public interface ICountryPayrollConfigRepository
{
    Task<CountryPayrollConfig?> GetAsync(string countryCode);
}
