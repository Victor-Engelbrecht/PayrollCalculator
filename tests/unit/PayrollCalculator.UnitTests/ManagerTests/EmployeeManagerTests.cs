using NSubstitute;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.UnitTests.Builders;
using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.UnitTests.ManagerTests;

[TestFixture]
public class EmployeeManagerTests
{
    private IEmployeeRepository   _employeeRepository = null!;
    private ICompanyRepository    _companyRepository  = null!;
    private IPayCalculationEngine _engine             = null!;
    private IPayrollRuleFactory   _ruleFactory        = null!;
    private IWideEventContext     _wideEvent          = null!;
    private EmployeeManager       _manager            = null!;

    [SetUp]
    public void SetUp()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();
        _companyRepository  = Substitute.For<ICompanyRepository>();
        _engine             = Substitute.For<IPayCalculationEngine>();
        _ruleFactory        = Substitute.For<IPayrollRuleFactory>();
        _wideEvent          = Substitute.For<IWideEventContext>();
        _manager            = new EmployeeManager(
            _employeeRepository, _companyRepository, _engine, _ruleFactory, _wideEvent);
    }

    [Test]
    public async Task Given_ExistingEmployeeId_When_GetEmployeeAsync_Then_EmployeeIsReturned()
    {
        // Given
        var employee = new EmployeeBuilder().WithId(10).Build();
        _employeeRepository.GetByIdAsync(10).Returns(employee);

        // When
        var result = await _manager.GetEmployeeAsync(10);

        // Then
        Assert.That(result, Is.SameAs(employee));
    }

    [Test]
    public async Task Given_NonExistingEmployeeId_When_GetEmployeeAsync_Then_NullIsReturned()
    {
        // Given
        _employeeRepository.GetByIdAsync(Arg.Any<int>()).Returns((Employee?)null);

        // When
        var result = await _manager.GetEmployeeAsync(99);

        // Then
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Given_CompanyId_When_GetEmployeesByCompanyAsync_Then_EmployeesAreReturned()
    {
        // Given
        var employees = new[] { new EmployeeBuilder().WithId(1).Build(), new EmployeeBuilder().WithId(2).Build() };
        _employeeRepository.GetByCompanyIdAsync(1).Returns(employees);

        // When
        var result = await _manager.GetEmployeesByCompanyAsync(1);

        // Then
        Assert.That(result, Is.EquivalentTo(employees));
    }

    [Test]
    public async Task Given_NewEmployee_When_CreateEmployeeAsync_Then_NewIdIsReturned()
    {
        // Given
        var employee = new EmployeeBuilder().Build();
        _employeeRepository.CreateAsync(employee).Returns(15);

        // When
        var id = await _manager.CreateEmployeeAsync(employee);

        // Then
        Assert.That(id, Is.EqualTo(15));
    }

    [Test]
    public async Task Given_NewEmployee_When_CreateEmployeeAsync_Then_RepositoryCreateIsInvoked()
    {
        // Given
        var employee = new EmployeeBuilder().Build();
        _employeeRepository.CreateAsync(employee).Returns(1);

        // When
        await _manager.CreateEmployeeAsync(employee);

        // Then
        await _employeeRepository.Received(1).CreateAsync(employee);
    }

    [Test]
    public async Task Given_ExistingEmployee_When_UpdateEmployeeAsync_Then_RepositoryUpdateIsInvoked()
    {
        // Given
        var employee = new EmployeeBuilder().Build();

        // When
        await _manager.UpdateEmployeeAsync(employee);

        // Then
        await _employeeRepository.Received(1).UpdateAsync(employee);
    }

    [Test]
    public async Task Given_ExistingEmployeeId_When_DeleteEmployeeAsync_Then_RepositoryDeleteIsInvoked()
    {
        // Given
        const int employeeId = 8;

        // When
        await _manager.DeleteEmployeeAsync(employeeId);

        // Then
        await _employeeRepository.Received(1).DeleteAsync(employeeId);
    }

    [Test]
    public async Task Given_ValidEmployeeId_When_CalculatePayAsync_Then_CalculationResultIsReturned()
    {
        // Given
        var employee = new EmployeeBuilder().WithId(1).WithCompanyId(1).Build();
        var company  = new CompanyBuilder().WithId(1).Build();
        var expected = new PayCalculationResultBuilder().Build();

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _companyRepository.GetByIdAsync(1).Returns(company);
        _ruleFactory.GetRulesAsync(company, employee).Returns(Array.Empty<IPayrollRule>());
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>()).Returns(expected);

        // When
        var result = await _manager.CalculatePayAsync(1);

        // Then
        Assert.That(result, Is.SameAs(expected));
    }

    [Test]
    public void Given_NonExistingEmployeeId_When_CalculatePayAsync_Then_InvalidOperationExceptionIsThrown()
    {
        // Given
        _employeeRepository.GetByIdAsync(Arg.Any<int>()).Returns((Employee?)null);

        // When / Then
        Assert.ThrowsAsync<InvalidOperationException>(() => _manager.CalculatePayAsync(99));
    }

    [Test]
    public void Given_EmployeeWithNonExistingCompany_When_CalculatePayAsync_Then_InvalidOperationExceptionIsThrown()
    {
        // Given
        var employee = new EmployeeBuilder().WithCompanyId(99).Build();
        _employeeRepository.GetByIdAsync(Arg.Any<int>()).Returns(employee);
        _companyRepository.GetByIdAsync(99).Returns((Company?)null);

        // When / Then
        Assert.ThrowsAsync<InvalidOperationException>(() => _manager.CalculatePayAsync(employee.Id));
    }

    [Test]
    public async Task Given_ValidEmployee_When_CalculatePayAsync_Then_RuleFactoryCalledWithCorrectCompanyAndEmployee()
    {
        // Given
        var employee = new EmployeeBuilder().WithId(1).WithCompanyId(1).Build();
        var company  = new CompanyBuilder().WithId(1).Build();

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _companyRepository.GetByIdAsync(1).Returns(company);
        _ruleFactory.GetRulesAsync(Arg.Any<Company>(), Arg.Any<Employee>()).Returns(Array.Empty<IPayrollRule>());
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>()).Returns(new PayCalculationResultBuilder().Build());

        // When
        await _manager.CalculatePayAsync(1);

        // Then
        await _ruleFactory.Received(1).GetRulesAsync(company, employee);
    }

    [Test]
    public async Task Given_ValidEmployee_When_CalculatePayAsync_Then_EngineIsCalledWithRulesFromFactory()
    {
        // Given
        var employee = new EmployeeBuilder().WithId(1).WithCompanyId(1).Build();
        var company  = new CompanyBuilder().WithId(1).Build();
        var rules    = new IPayrollRule[] { Substitute.For<IPayrollRule>() };

        _employeeRepository.GetByIdAsync(1).Returns(employee);
        _companyRepository.GetByIdAsync(1).Returns(company);
        _ruleFactory.GetRulesAsync(company, employee).Returns(rules);
        _engine.Calculate(Arg.Any<IEnumerable<IPayrollRule>>()).Returns(new PayCalculationResultBuilder().Build());

        // When
        await _manager.CalculatePayAsync(1);

        // Then
        _engine.Received(1).Calculate(rules);
    }
}
