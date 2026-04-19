using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class CompanyPayrollConfigBuilder
{
    private int     _companyId = 1;
    private decimal _taxRate   = 0.20m;

    public CompanyPayrollConfigBuilder WithCompanyId(int id)        { _companyId = id;      return this; }
    public CompanyPayrollConfigBuilder WithTaxRate(decimal taxRate) { _taxRate   = taxRate; return this; }

    public CompanyPayrollConfig Build() => new()
    {
        CompanyId = _companyId,
        TaxRate   = _taxRate
    };
}
