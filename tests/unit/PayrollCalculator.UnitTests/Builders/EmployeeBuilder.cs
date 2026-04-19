using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class EmployeeBuilder
{
    private int      _id                = 1;
    private int      _companyId         = 1;
    private string   _firstName         = "Jane";
    private string   _lastName          = "Doe";
    private string   _email             = "jane.doe@example.com";
    private decimal  _baseSalary        = 5000m;
    private string?  _bankAccountNumber = null;
    private string?  _countryCode       = null;

    public EmployeeBuilder WithId(int id)                              { _id                = id;                return this; }
    public EmployeeBuilder WithCompanyId(int companyId)               { _companyId         = companyId;        return this; }
    public EmployeeBuilder WithFirstName(string firstName)            { _firstName         = firstName;        return this; }
    public EmployeeBuilder WithLastName(string lastName)              { _lastName          = lastName;         return this; }
    public EmployeeBuilder WithEmail(string email)                    { _email             = email;            return this; }
    public EmployeeBuilder WithBaseSalary(decimal salary)             { _baseSalary        = salary;           return this; }
    public EmployeeBuilder WithBankAccountNumber(string? account)     { _bankAccountNumber = account;          return this; }
    public EmployeeBuilder WithCountryCode(string? countryCode)       { _countryCode       = countryCode;      return this; }

    public Employee Build() => new()
    {
        Id                = _id,
        CompanyId         = _companyId,
        FirstName         = _firstName,
        LastName          = _lastName,
        Email             = _email,
        BaseSalary        = _baseSalary,
        BankAccountNumber = _bankAccountNumber,
        CountryCode       = _countryCode
    };
}
