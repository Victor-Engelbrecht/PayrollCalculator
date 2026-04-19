using NSubstitute;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Contracts;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class PayrollRuleFactoryTests
{
    [Test]
    public async Task Given_NoProviders_When_GetRulesAsync_Then_EmptyListIsReturned()
    {
        // Given
        var factory = new PayrollRuleFactory([]);

        // When
        var rules = await factory.GetRulesAsync(new CompanyBuilder().Build(), new EmployeeBuilder().Build());

        // Then
        Assert.That(rules, Is.Empty);
    }

    [Test]
    public async Task Given_SingleProvider_When_GetRulesAsync_Then_ProviderRulesAreReturned()
    {
        // Given
        var provider = Substitute.For<IPayrollRuleProvider>();
        var expected = new IPayrollRule[] { new BaseSalaryRule(5000m) };
        provider.GetRulesAsync(Arg.Any<PayrollRuleContext>()).Returns(expected);

        var factory = new PayrollRuleFactory([provider]);

        // When
        var rules = await factory.GetRulesAsync(new CompanyBuilder().Build(), new EmployeeBuilder().Build());

        // Then
        Assert.That(rules, Has.Count.EqualTo(1));
        Assert.That(rules[0], Is.SameAs(expected[0]));
    }

    [Test]
    public async Task Given_MultipleProviders_When_GetRulesAsync_Then_AllProviderRulesAreAggregated()
    {
        // Given
        var providerA = Substitute.For<IPayrollRuleProvider>();
        var providerB = Substitute.For<IPayrollRuleProvider>();

        providerA.GetRulesAsync(Arg.Any<PayrollRuleContext>())
                 .Returns(new IPayrollRule[] { new BaseSalaryRule(5000m) });
        providerB.GetRulesAsync(Arg.Any<PayrollRuleContext>())
                 .Returns(new IPayrollRule[] { new FlatTaxRule(0.20m), new MinimumWageRule(1500m) });

        var factory = new PayrollRuleFactory([providerA, providerB]);

        // When
        var rules = await factory.GetRulesAsync(new CompanyBuilder().Build(), new EmployeeBuilder().Build());

        // Then
        Assert.That(rules, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task Given_SingleProvider_When_GetRulesAsync_Then_ContextContainsCorrectCompany()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().Build();

        var provider = Substitute.For<IPayrollRuleProvider>();
        provider.GetRulesAsync(Arg.Any<PayrollRuleContext>())
                .Returns(Enumerable.Empty<IPayrollRule>());

        var factory = new PayrollRuleFactory([provider]);

        // When
        await factory.GetRulesAsync(company, employee);

        // Then
        await provider.Received(1)
                      .GetRulesAsync(Arg.Is<PayrollRuleContext>(c => c.Company == company));
    }

    [Test]
    public async Task Given_SingleProvider_When_GetRulesAsync_Then_ContextContainsCorrectEmployee()
    {
        // Given
        var company  = new CompanyBuilder().Build();
        var employee = new EmployeeBuilder().Build();

        var provider = Substitute.For<IPayrollRuleProvider>();
        provider.GetRulesAsync(Arg.Any<PayrollRuleContext>())
                .Returns(Enumerable.Empty<IPayrollRule>());

        var factory = new PayrollRuleFactory([provider]);

        // When
        await factory.GetRulesAsync(company, employee);

        // Then
        await provider.Received(1)
                      .GetRulesAsync(Arg.Is<PayrollRuleContext>(c => c.Employee == employee));
    }

    [Test]
    public async Task Given_SingleProviderReturningNoRules_When_GetRulesAsync_Then_EmptyListIsReturned()
    {
        // Given
        var provider = Substitute.For<IPayrollRuleProvider>();
        provider.GetRulesAsync(Arg.Any<PayrollRuleContext>())
                .Returns(Enumerable.Empty<IPayrollRule>());

        var factory = new PayrollRuleFactory([provider]);

        // When
        var rules = await factory.GetRulesAsync(new CompanyBuilder().Build(), new EmployeeBuilder().Build());

        // Then
        Assert.That(rules, Is.Empty);
    }
}
