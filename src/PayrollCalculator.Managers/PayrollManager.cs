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

    public Task<PayrollSummary> RunPayrollAsync(int companyId, DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();

        // Intended persistence shape once repositories are implemented:
        //
        // var employees = await _employeeRepository.GetByCompanyIdAsync(companyId);
        // var payrollId = await _payrollRepository.CreateAsync(new Payroll { ... });
        //
        // foreach (var employee in employees)
        // {
        //     var rules  = /* build rules for employee */;
        //     var result = _payCalculationEngine.Calculate(employee, rules);
        //     var now    = DateTime.UtcNow;
        //
        //     await _payrollRepository.CreatePayslipAsync(new PayslipDetail { ... });
        //
        //     foreach (var item in result.LineItems)
        //     {
        //         if (item.Kind == PayslipLineItemKind.Addition)
        //             await _payrollRepository.CreateAdditionAsync(new Addition
        //             {
        //                 EmployeeId  = employee.Id,
        //                 PayrollId   = payrollId,
        //                 Description = item.Description,
        //                 Amount      = item.Amount,
        //                 CreatedAt   = now,
        //                 UpdatedAt   = now
        //             });
        //         else
        //             await _payrollRepository.CreateDeductionAsync(new Deduction
        //             {
        //                 EmployeeId  = employee.Id,
        //                 PayrollId   = payrollId,
        //                 Description = item.Description,
        //                 Amount      = item.Amount,
        //                 CreatedAt   = now,
        //                 UpdatedAt   = now
        //             });
        //     }
        // }
    }
    public Task<Payroll?> GetPayrollAsync(int payrollId) => throw new NotImplementedException();
    public Task<IEnumerable<PayslipDetail>> GetPayslipsAsync(int payrollId) => throw new NotImplementedException();
}
