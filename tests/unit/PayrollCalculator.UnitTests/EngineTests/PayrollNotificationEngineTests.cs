using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class PayrollNotificationEngineTests
{
    private PayrollNotificationEngine _engine = null!;

    [SetUp]
    public void SetUp() => _engine = new PayrollNotificationEngine();

    [Test]
    public void Given_EmployeeWithValidEmail_When_BuildNotifications_Then_NotificationIsYielded()
    {
        // Given
        var payslips = new[] { new EmployeePayslipBuilder().Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public void Given_EmployeeWithNullEmail_When_BuildNotifications_Then_NotificationIsSkipped()
    {
        // Given
        var employee = new EmployeeBuilder().WithEmail(null!).Build();
        var payslips = new[] { new EmployeePayslipBuilder().WithEmployee(employee).Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Given_EmployeeWithEmptyEmail_When_BuildNotifications_Then_NotificationIsSkipped()
    {
        // Given
        var payslips = new[] { new EmployeePayslipBuilder().WithEmployeeEmail(string.Empty).Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Given_EmployeeWithWhitespaceEmail_When_BuildNotifications_Then_NotificationIsSkipped()
    {
        // Given
        var payslips = new[] { new EmployeePayslipBuilder().WithEmployeeEmail("   ").Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void Given_EmployeeWithValidEmail_When_BuildNotifications_Then_RecipientNameIsFirstNameAndLastName()
    {
        // Given
        var employee = new EmployeeBuilder().WithFirstName("Jane").WithLastName("Doe").Build();
        var payslips = new[] { new EmployeePayslipBuilder().WithEmployee(employee).Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result[0].RecipientName, Is.EqualTo("Jane Doe"));
    }

    [Test]
    public void Given_EmployeeWithValidEmail_When_BuildNotifications_Then_RecipientEmailMatchesEmployeeEmail()
    {
        // Given
        var employee = new EmployeeBuilder().WithEmail("jane.doe@example.com").Build();
        var payslips = new[] { new EmployeePayslipBuilder().WithEmployee(employee).Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result[0].RecipientEmail, Is.EqualTo("jane.doe@example.com"));
    }

    [Test]
    public void Given_EmployeeWithValidEmail_When_BuildNotifications_Then_PayslipIsPassedThrough()
    {
        // Given
        var payslip  = new PayslipDetailBuilder().Build();
        var payslips = new[] { new EmployeePayslipBuilder().WithPayslip(payslip).Build() };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result[0].Payslip, Is.SameAs(payslip));
    }

    [Test]
    public void Given_MultipleEmployeesWithMixedEmails_When_BuildNotifications_Then_OnlyEmployeesWithEmailsReceiveNotifications()
    {
        // Given
        var payslips = new[]
        {
            new EmployeePayslipBuilder().WithEmployeeEmail("alice@example.com").Build(),
            new EmployeePayslipBuilder().WithEmployeeEmail(string.Empty).Build(),
            new EmployeePayslipBuilder().WithEmployeeEmail("bob@example.com").Build(),
            new EmployeePayslipBuilder().WithEmployeeEmail("   ").Build()
        };

        // When
        var result = _engine.BuildNotifications(payslips).ToList();

        // Then
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.Select(n => n.RecipientEmail),
            Is.EquivalentTo(new[] { "alice@example.com", "bob@example.com" }));
    }
}
