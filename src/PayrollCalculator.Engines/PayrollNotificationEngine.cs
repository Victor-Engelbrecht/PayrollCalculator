using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;

namespace PayrollCalculator.Engines;

public class PayrollNotificationEngine : IPayrollNotificationEngine
{
    public IEnumerable<PayslipNotification> BuildNotifications(IEnumerable<EmployeePayslip> employeePayslips)
    {
        foreach (var ep in employeePayslips)
        {
            if (string.IsNullOrWhiteSpace(ep.Employee.Email))
                continue;

            yield return new PayslipNotification
            {
                RecipientEmail = ep.Employee.Email,
                RecipientName  = $"{ep.Employee.FirstName} {ep.Employee.LastName}",
                Payslip        = ep.Payslip
            };
        }
    }
}
