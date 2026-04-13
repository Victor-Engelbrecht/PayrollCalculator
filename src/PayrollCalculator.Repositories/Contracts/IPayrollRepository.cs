using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories.Contracts;

public interface IPayrollRepository
{
    Task<Payroll?> GetByIdAsync(int id);
    Task<IEnumerable<Payroll>> GetByCompanyIdAsync(int companyId);
    Task<int> CreateAsync(Payroll payroll);
    Task UpdateStatusAsync(int payrollId, string status);

    Task<PayslipDetail?> GetPayslipByIdAsync(int id);
    Task<IEnumerable<PayslipDetail>> GetPayslipsByPayrollIdAsync(int payrollId);
    Task<int> CreatePayslipAsync(PayslipDetail payslip);
    Task UpdatePayslipPaymentReferenceAsync(int payslipId, string paymentReference);

    Task<int> CreateAdditionAsync(Addition addition);
    Task<IEnumerable<Addition>> GetAdditionsByPayslipAsync(int payrollId, int employeeId);

    Task<int> CreateDeductionAsync(Deduction deduction);
    Task<IEnumerable<Deduction>> GetDeductionsByPayslipAsync(int payrollId, int employeeId);
}
