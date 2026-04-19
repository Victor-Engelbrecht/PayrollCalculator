using NSubstitute;
using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Domain.Models.Adapters;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.UnitTests.Builders;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.UnitTests.ManagerTests;

[TestFixture]
public class PayrollManagerTests
{
    private IPayCalculationEngine      _engine             = null!;
    private IPayrollNotificationEngine _notificationEngine = null!;
    private ICompanyRepository         _companyRepository  = null!;
    private IEmployeeRepository        _employeeRepository = null!;
    private IPayrollRepository         _payrollRepository  = null!;
    private IPaymentAdapter            _paymentAdapter     = null!;
    private IEmailAdapter              _emailAdapter       = null!;
    private IPayrollRuleFactory        _ruleFactory        = null!;
    private IWideEventContext          _wideEvent          = null!;
    private PayrollManager             _manager            = null!;

    [SetUp]
    public void SetUp()
    {
        _engine             = Substitute.For<IPayCalculationEngine>();
        _notificationEngine = Substitute.For<IPayrollNotificationEngine>();
        _companyRepository  = Substitute.For<ICompanyRepository>();
        _employeeRepository = Substitute.For<IEmployeeRepository>();
        _payrollRepository  = Substitute.For<IPayrollRepository>();
        _paymentAdapter     = Substitute.For<IPaymentAdapter>();
        _emailAdapter       = Substitute.For<IEmailAdapter>();
        _ruleFactory        = Substitute.For<IPayrollRuleFactory>();
        _wideEvent          = Substitute.For<IWideEventContext>();

        _manager = new PayrollManager(
            _engine, _notificationEngine,
            _companyRepository, _employeeRepository, _payrollRepository,
            _paymentAdapter, _emailAdapter,
            _ruleFactory, _wideEvent);

        _ruleFactory.GetRulesAsync(Arg.Any<Company>(), Arg.Any<Employee>())
                    .Returns(Array.Empty<IPayrollRule>());
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>())
               .Returns(new PayCalculationResultBuilder().Build());
        _notificationEngine.BuildNotifications(Arg.Any<IEnumerable<EmployeePayslip>>())
                           .Returns([]);
        _payrollRepository.CreateAsync(Arg.Any<Payroll>()).Returns(1);
        _payrollRepository.CreatePayslipAsync(Arg.Any<PayslipDetail>()).Returns(1);
        _paymentAdapter.ProcessPaymentAsync(Arg.Any<PaymentRequest>())
                       .Returns(new PaymentResult { Success = true, ProviderReference = "REF-001" });
    }

    [Test]
    public void Given_NonExistingCompanyId_When_RunPayrollAsync_Then_InvalidOperationExceptionIsThrown()
    {
        // Given
        _companyRepository.GetByIdAsync(Arg.Any<int>()).Returns((Company?)null);

        // When / Then
        Assert.ThrowsAsync<InvalidOperationException>(() => _manager.RunPayrollAsync(99));
    }

    [Test]
    public async Task Given_CompanyWithNoEmployees_When_RunPayrollAsync_Then_SummaryHasZeroEmployeeCount()
    {
        // Given
        var company = new CompanyBuilder().Build();
        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([]);

        // When
        var summary = await _manager.RunPayrollAsync(company.Id);

        // Then
        Assert.That(summary.EmployeeCount, Is.EqualTo(0));
    }

    [Test]
    public async Task Given_CompanyWithNoEmployees_When_RunPayrollAsync_Then_SummaryHasZeroTotalNetPaid()
    {
        // Given
        var company = new CompanyBuilder().Build();
        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([]);

        // When
        var summary = await _manager.RunPayrollAsync(company.Id);

        // Then
        Assert.That(summary.TotalNetPaid, Is.EqualTo(0m));
    }

    [Test]
    public async Task Given_CompanyWithEmployees_When_RunPayrollAsync_Then_SummaryEmployeeCountMatchesEmployeeCount()
    {
        // Given
        var company = new CompanyBuilder().Build();
        var employees = new[]
        {
            new EmployeeBuilder().WithId(1).Build(),
            new EmployeeBuilder().WithId(2).Build()
        };
        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns(employees);

        // When
        var summary = await _manager.RunPayrollAsync(company.Id);

        // Then
        Assert.That(summary.EmployeeCount, Is.EqualTo(2));
    }

    [Test]
    public async Task Given_CompanyWithEmployees_When_RunPayrollAsync_Then_SummaryTotalNetPaidIsAggregated()
    {
        // Given
        var company = new CompanyBuilder().Build();
        var employees = new[]
        {
            new EmployeeBuilder().WithId(1).Build(),
            new EmployeeBuilder().WithId(2).Build()
        };
        var calcResult = new PayCalculationResultBuilder().WithNetAmount(3000m).Build();

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns(employees);
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>()).Returns(calcResult);

        // When
        var summary = await _manager.RunPayrollAsync(company.Id);

        // Then
        Assert.That(summary.TotalNetPaid, Is.EqualTo(6000m));
    }

    [Test]
    public async Task Given_CompanyWithEmployees_When_RunPayrollAsync_Then_PayslipIsCreatedForEachEmployee()
    {
        // Given
        var company = new CompanyBuilder().Build();
        var employees = new[]
        {
            new EmployeeBuilder().WithId(1).Build(),
            new EmployeeBuilder().WithId(2).Build(),
            new EmployeeBuilder().WithId(3).Build()
        };
        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns(employees);

        // When
        await _manager.RunPayrollAsync(company.Id);

        // Then
        await _payrollRepository.Received(3).CreatePayslipAsync(Arg.Any<PayslipDetail>());
    }

    [Test]
    public async Task Given_EmployeeWithBankAccount_When_RunPayrollAsync_Then_PaymentIsProcessed()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().WithBankAccountNumber("ACC-123").Build();

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([employee]);

        // When
        await _manager.RunPayrollAsync(company.Id);

        // Then
        await _paymentAdapter.Received(1).ProcessPaymentAsync(Arg.Any<PaymentRequest>());
    }

    [Test]
    public async Task Given_EmployeeWithoutBankAccount_When_RunPayrollAsync_Then_PaymentIsSkipped()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().WithBankAccountNumber(null).Build();

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([employee]);

        // When
        await _manager.RunPayrollAsync(company.Id);

        // Then
        await _paymentAdapter.DidNotReceive().ProcessPaymentAsync(Arg.Any<PaymentRequest>());
    }

    [Test]
    public async Task Given_CompanyWithEmployees_When_RunPayrollAsync_Then_PayrollStatusUpdatedToCompleted()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().Build();

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([employee]);
        _payrollRepository.CreateAsync(Arg.Any<Payroll>()).Returns(5);

        // When
        await _manager.RunPayrollAsync(company.Id);

        // Then
        await _payrollRepository.Received(1).UpdateStatusAsync(5, "Completed");
    }

    [Test]
    public async Task Given_CompanyWithEmployees_When_RunPayrollAsync_Then_EmailsAreSent()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().Build();
        var payslip  = new PayslipDetailBuilder().Build();
        var notification = new PayslipNotification
        {
            RecipientEmail = employee.Email,
            RecipientName  = $"{employee.FirstName} {employee.LastName}",
            Payslip        = payslip
        };

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([employee]);
        _notificationEngine.BuildNotifications(Arg.Any<IEnumerable<EmployeePayslip>>())
                           .Returns([notification]);

        // When
        await _manager.RunPayrollAsync(company.Id);

        // Then
        await _emailAdapter.Received(1).SendPayslipAsync(Arg.Any<PayslipEmailRequest>());
    }

    [Test]
    public async Task Given_CompanyWithViolations_When_RunPayrollAsync_Then_ViolationsAreInSummary()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().Build();
        var violations = new List<RuleViolation> { new() { RuleName = "MinimumWageRule", Message = "Below minimum." } };
        var calcResult = new PayCalculationResultBuilder().WithViolations(violations).Build();

        _companyRepository.GetByIdAsync(company.Id).Returns(company);
        _employeeRepository.GetByCompanyIdAsync(company.Id).Returns([employee]);
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>()).Returns(calcResult);

        // When
        var summary = await _manager.RunPayrollAsync(company.Id);

        // Then
        Assert.That(summary.Violations, Has.Count.EqualTo(1));
        Assert.That(summary.Violations[0].RuleName, Is.EqualTo("MinimumWageRule"));
    }

    [Test]
    public async Task Given_ExistingPayrollId_When_GetPayrollAsync_Then_PayrollIsReturned()
    {
        // Given
        var payroll = new PayrollBuilder().WithId(3).Build();
        _payrollRepository.GetByIdAsync(3).Returns(payroll);

        // When
        var result = await _manager.GetPayrollAsync(3);

        // Then
        Assert.That(result, Is.SameAs(payroll));
    }

    [Test]
    public async Task Given_ExistingPayrollId_When_GetPayslipsAsync_Then_PayslipsAreReturned()
    {
        // Given
        var payslips = new[] { new PayslipDetailBuilder().WithPayrollId(3).Build() };
        _payrollRepository.GetPayslipsByPayrollIdAsync(3).Returns(payslips);

        // When
        var result = await _manager.GetPayslipsAsync(3);

        // Then
        Assert.That(result, Is.EquivalentTo(payslips));
    }
}
