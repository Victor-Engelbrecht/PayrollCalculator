using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class PayrollRuleContextBuilder
{
    private Company  _company  = new CompanyBuilder().Build();
    private Employee _employee = new EmployeeBuilder().Build();

    public PayrollRuleContextBuilder WithCompany(Company company)   { _company  = company;  return this; }
    public PayrollRuleContextBuilder WithEmployee(Employee employee) { _employee = employee; return this; }

    public PayrollRuleContext Build() => new()
    {
        Company  = _company,
        Employee = _employee
    };
}
