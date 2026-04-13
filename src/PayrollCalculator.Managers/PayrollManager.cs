using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Managers;

public class PayrollManager : IPayrollManager
{
    private readonly IPayCalculationEngine _payCalculationEngine;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IPaymentAdapter _paymentAdapter;
    private readonly IEmailAdapter _emailAdapter;

    public PayrollManager(
        IPayCalculationEngine payCalculationEngine,
        IEmployeeRepository employeeRepository,
        IPayrollRepository payrollRepository,
        IPaymentAdapter paymentAdapter,
        IEmailAdapter emailAdapter)
    {
        _payCalculationEngine = payCalculationEngine;
        _employeeRepository = employeeRepository;
        _payrollRepository = payrollRepository;
        _paymentAdapter = paymentAdapter;
        _emailAdapter = emailAdapter;
    }

    public Task<PayrollSummary> RunPayrollAsync(int companyId, DateTime periodStart, DateTime periodEnd) => throw new NotImplementedException();
    public Task<Payroll?> GetPayrollAsync(int payrollId) => throw new NotImplementedException();
    public Task<IEnumerable<PayslipDetail>> GetPayslipsAsync(int payrollId) => throw new NotImplementedException();
}
