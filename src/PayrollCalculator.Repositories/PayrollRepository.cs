using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Repositories;

public class PayrollRepository : IPayrollRepository
{
    public Task<Payroll?> GetByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<Payroll>> GetByCompanyIdAsync(int companyId) => throw new NotImplementedException();
    public Task<int> CreateAsync(Payroll payroll) => throw new NotImplementedException();
    public Task UpdateStatusAsync(int payrollId, string status) => throw new NotImplementedException();

    public Task<PayslipDetail?> GetPayslipByIdAsync(int id) => throw new NotImplementedException();
    public Task<IEnumerable<PayslipDetail>> GetPayslipsByPayrollIdAsync(int payrollId) => throw new NotImplementedException();
    public Task<int> CreatePayslipAsync(PayslipDetail payslip) => throw new NotImplementedException();
    public Task UpdatePayslipPaymentReferenceAsync(int payslipId, string paymentReference) => throw new NotImplementedException();
}
