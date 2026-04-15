using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Domain.Models.Adapters;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.Contracts;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.Managers;

public class PayrollManager : IPayrollManager
{
    private readonly IPayCalculationEngine _payCalculationEngine;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IPaymentAdapter _paymentAdapter;
    private readonly IEmailAdapter _emailAdapter;
    private readonly PayrollRuleFactory _ruleFactory;
    private readonly IWideEventContext _wideEvent;

    public PayrollManager(
        IPayCalculationEngine payCalculationEngine,
        IEmployeeRepository employeeRepository,
        IPayrollRepository payrollRepository,
        IPaymentAdapter paymentAdapter,
        IEmailAdapter emailAdapter,
        PayrollRuleFactory ruleFactory,
        IWideEventContext wideEvent)
    {
        _payCalculationEngine = payCalculationEngine;
        _employeeRepository = employeeRepository;
        _payrollRepository = payrollRepository;
        _paymentAdapter = paymentAdapter;
        _emailAdapter = emailAdapter;
        _ruleFactory = ruleFactory;
        _wideEvent = wideEvent;
    }

    public async Task<PayrollSummary> RunPayrollAsync(int companyId, DateTime periodStart, DateTime periodEnd)
    {
        _wideEvent.Set("operation", "payroll.run");
        _wideEvent.Set("company_id", companyId);
        _wideEvent.Set("period_start", periodStart);
        _wideEvent.Set("period_end", periodEnd);

        var employees = (await _employeeRepository.GetByCompanyIdAsync(companyId)).ToList();
        _wideEvent.Set("employee_count", employees.Count);

        var payrollId = await _payrollRepository.CreateAsync(new Payroll
        {
            CompanyId = companyId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            RunAt = DateTime.UtcNow,
            Status = "Running"
        });
        _wideEvent.Set("payroll_id", payrollId);

        var allViolations = new List<RuleViolation>();
        decimal totalNetPaid = 0;
        decimal totalAdditions = 0;
        decimal totalDeductions = 0;
        decimal totalGross = 0;
        var payslipsGenerated = 0;
        var paymentFailures = 0;
        var paymentsProcessed = 0;
        var emailFailures = 0;
        var emailsSent = 0;

        foreach (var employee in employees)
        {
            var rules = _ruleFactory.GetRules(employee);
            var result = _payCalculationEngine.Calculate(employee, rules);

            var payslipId = await _payrollRepository.CreatePayslipAsync(new PayslipDetail
            {
                PayrollId = payrollId,
                EmployeeId = employee.Id,
                BaseSalary = employee.BaseSalary,
                TotalAdditions = result.TotalAdditions,
                TotalDeductions = result.TotalDeductions,
                NetAmount = result.NetAmount
            });
            payslipsGenerated++;

            foreach (var item in result.LineItems)
            {
                if (item.Kind == PayslipLineItemKind.Addition)
                    await _payrollRepository.CreateAdditionAsync(new Addition
                    {
                        EmployeeId = employee.Id,
                        PayrollId = payrollId,
                        Description = item.Description,
                        Amount = item.Amount
                    });
                else
                    await _payrollRepository.CreateDeductionAsync(new Deduction
                    {
                        EmployeeId = employee.Id,
                        PayrollId = payrollId,
                        Description = item.Description,
                        Amount = item.Amount
                    });
            }

            if (!string.IsNullOrEmpty(employee.BankAccountNumber))
            {
                var payment = await _paymentAdapter.ProcessPaymentAsync(new PaymentRequest
                {
                    PayslipDetailId = payslipId,
                    RecipientBankAccount = employee.BankAccountNumber,
                    Amount = result.NetAmount,
                    Reference = $"PAYROLL-{payrollId}-EMP-{employee.Id}"
                });

                paymentsProcessed++;
                if (payment.Success && payment.ProviderReference != null)
                    await _payrollRepository.UpdatePayslipPaymentReferenceAsync(payslipId, payment.ProviderReference);
                else
                    paymentFailures++;
            }

            try
            {
                await _emailAdapter.SendPayslipAsync(new PayslipEmailRequest
                {
                    RecipientEmail = employee.Email,
                    RecipientName = $"{employee.FirstName} {employee.LastName}",
                    Payslip = new PayslipDetail
                    {
                        Id = payslipId,
                        PayrollId = payrollId,
                        EmployeeId = employee.Id,
                        BaseSalary = employee.BaseSalary,
                        TotalAdditions = result.TotalAdditions,
                        TotalDeductions = result.TotalDeductions,
                        NetAmount = result.NetAmount
                    }
                });
                emailsSent++;
            }
            catch
            {
                emailFailures++;
                throw;
            }

            allViolations.AddRange(result.Violations);
            totalNetPaid += result.NetAmount;
            totalAdditions += result.TotalAdditions;
            totalDeductions += result.TotalDeductions;
            totalGross += employee.BaseSalary + result.TotalAdditions;
        }

        await _payrollRepository.UpdateStatusAsync(payrollId, "Completed");

        _wideEvent.Set("payroll_status", "Completed");
        _wideEvent.Set("payslips_generated", payslipsGenerated);
        _wideEvent.Set("total_gross", totalGross);
        _wideEvent.Set("total_additions", totalAdditions);
        _wideEvent.Set("total_deductions", totalDeductions);
        _wideEvent.Set("total_net_paid", totalNetPaid);
        _wideEvent.Set("rule_violations_count", allViolations.Count);
        _wideEvent.Set("rule_violations", allViolations.Select(v => v.RuleName).Distinct().ToArray());
        _wideEvent.Set("payments_processed", paymentsProcessed);
        _wideEvent.Set("payment_failures", paymentFailures);
        _wideEvent.Set("emails_sent", emailsSent);
        _wideEvent.Set("email_failures", emailFailures);

        return new PayrollSummary
        {
            PayrollId = payrollId,
            EmployeeCount = employees.Count,
            TotalNetPaid = totalNetPaid,
            Violations = allViolations,
            CompletedAt = DateTime.UtcNow
        };
    }

    public Task<Payroll?> GetPayrollAsync(int payrollId)
    {
        _wideEvent.Set("operation", "payroll.get");
        _wideEvent.Set("payroll_id", payrollId);
        return _payrollRepository.GetByIdAsync(payrollId);
    }

    public Task<IEnumerable<PayslipDetail>> GetPayslipsAsync(int payrollId)
    {
        _wideEvent.Set("operation", "payroll.get_payslips");
        _wideEvent.Set("payroll_id", payrollId);
        return _payrollRepository.GetPayslipsByPayrollIdAsync(payrollId);
    }
}
