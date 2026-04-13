using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Managers.Contracts;

public interface IPayrollManager
{
    Task<PayrollSummary> RunPayrollAsync(int companyId, DateTime periodStart, DateTime periodEnd);
    Task<Payroll?> GetPayrollAsync(int payrollId);
    Task<IEnumerable<PayslipDetail>> GetPayslipsAsync(int payrollId);
}
