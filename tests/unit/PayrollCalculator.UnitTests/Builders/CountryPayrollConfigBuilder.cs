using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class CountryPayrollConfigBuilder
{
    private string  _countryCode  = "ZA";
    private decimal _minimumWage  = 1500m;

    public CountryPayrollConfigBuilder WithCountryCode(string code)      { _countryCode = code;        return this; }
    public CountryPayrollConfigBuilder WithMinimumWage(decimal minWage)  { _minimumWage = minWage;     return this; }

    public CountryPayrollConfig Build() => new()
    {
        CountryCode  = _countryCode,
        MinimumWage  = _minimumWage
    };
}
