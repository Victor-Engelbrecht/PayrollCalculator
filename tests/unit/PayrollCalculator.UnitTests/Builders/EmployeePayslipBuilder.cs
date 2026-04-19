using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class EmployeePayslipBuilder
{
    private Employee      _employee = new EmployeeBuilder().Build();
    private PayslipDetail _payslip  = new PayslipDetailBuilder().Build();

    public EmployeePayslipBuilder WithEmployee(Employee employee)      { _employee = employee; return this; }
    public EmployeePayslipBuilder WithPayslip(PayslipDetail payslip)   { _payslip  = payslip;  return this; }

    public EmployeePayslipBuilder WithEmployeeEmail(string email)
    {
        _employee = new EmployeeBuilder()
            .WithFirstName(_employee.FirstName)
            .WithLastName(_employee.LastName)
            .WithEmail(email)
            .Build();
        return this;
    }

    public EmployeePayslip Build() => new()
    {
        Employee = _employee,
        Payslip  = _payslip
    };
}
