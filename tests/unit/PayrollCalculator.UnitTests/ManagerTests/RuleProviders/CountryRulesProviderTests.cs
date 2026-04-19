using NSubstitute;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.RuleProviders;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.ManagerTests.RuleProviders;

[TestFixture]
public class CountryRulesProviderTests
{
    private ICountryPayrollConfigRepository _configRepo = null!;
    private CountryRulesProvider            _provider   = null!;

    [SetUp]
    public void SetUp()
    {
        _configRepo = Substitute.For<ICountryPayrollConfigRepository>();
        _provider   = new CountryRulesProvider(_configRepo);
    }

    [Test]
    public async Task Given_EmployeeWithNullCountryCode_When_GetRulesAsync_Then_EmptyRulesAreReturned()
    {
        // Given
        var employee = new EmployeeBuilder().WithCountryCode(null).Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules, Is.Empty);
    }

    [Test]
    public async Task Given_EmployeeWithNullCountryCode_When_GetRulesAsync_Then_RepositoryIsNotQueried()
    {
        // Given
        var employee = new EmployeeBuilder().WithCountryCode(null).Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();

        // When
        await _provider.GetRulesAsync(context);

        // Then
        await _configRepo.DidNotReceive().GetAsync(Arg.Any<string>());
    }

    [Test]
    public async Task Given_EmployeeWithCountryCodeButNoConfig_When_GetRulesAsync_Then_EmptyRulesAreReturned()
    {
        // Given
        var employee = new EmployeeBuilder().WithCountryCode("ZA").Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();
        _configRepo.GetAsync("ZA").Returns((CountryPayrollConfig?)null);

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules, Is.Empty);
    }

    [Test]
    public async Task Given_EmployeeWithCountryCodeAndConfig_When_GetRulesAsync_Then_SingleRuleIsReturned()
    {
        // Given
        var employee = new EmployeeBuilder().WithCountryCode("ZA").Build();
        var config   = new CountryPayrollConfigBuilder().WithCountryCode("ZA").Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();
        _configRepo.GetAsync("ZA").Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task Given_EmployeeWithCountryCodeAndConfig_When_GetRulesAsync_Then_ReturnedRuleIsMinimumWageRule()
    {
        // Given
        var employee = new EmployeeBuilder().WithCountryCode("ZA").Build();
        var config   = new CountryPayrollConfigBuilder().WithCountryCode("ZA").Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();
        _configRepo.GetAsync("ZA").Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Single(), Is.InstanceOf<MinimumWageRule>());
    }

    [Test]
    public async Task Given_EmployeeWithCountryCodeAndConfig_When_GetRulesAsync_Then_MinimumWageRuleUsesConfiguredMinimum()
    {
        // Given
        var employee  = new EmployeeBuilder().WithCountryCode("ZA").Build();
        var config    = new CountryPayrollConfigBuilder().WithCountryCode("ZA").WithMinimumWage(2000m).Build();
        var context   = new PayrollRuleContextBuilder().WithEmployee(employee).Build();
        var calcCtx   = new PayCalculationContextBuilder().WithNetPay(1500m).Build();
        var violations = new List<RuleViolation>();
        _configRepo.GetAsync("ZA").Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);
        rules.Single().Apply(calcCtx, violations);

        // Then
        Assert.That(violations, Has.Count.EqualTo(1));
        Assert.That(violations[0].RuleName, Is.EqualTo(nameof(MinimumWageRule)));
    }
}
