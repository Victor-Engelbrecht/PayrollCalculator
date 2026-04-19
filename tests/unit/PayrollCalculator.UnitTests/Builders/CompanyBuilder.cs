using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class CompanyBuilder
{
    private int     _id                 = 1;
    private string  _name               = "Acme Corp";
    private string  _registrationNumber = "REG-001";
    private string? _contactEmail       = null;

    public CompanyBuilder WithId(int id)                              { _id                 = id;                 return this; }
    public CompanyBuilder WithName(string name)                       { _name               = name;               return this; }
    public CompanyBuilder WithRegistrationNumber(string regNumber)    { _registrationNumber = regNumber;          return this; }
    public CompanyBuilder WithContactEmail(string? email)             { _contactEmail       = email;              return this; }

    public Company Build() => new()
    {
        Id                 = _id,
        Name               = _name,
        RegistrationNumber = _registrationNumber,
        ContactEmail       = _contactEmail
    };
}
