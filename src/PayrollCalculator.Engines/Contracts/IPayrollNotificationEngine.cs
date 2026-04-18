using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Engines.Contracts;

public interface IPayrollNotificationEngine
{
    IEnumerable<PayslipNotification> BuildNotifications(IEnumerable<EmployeePayslip> employeePayslips);
}
