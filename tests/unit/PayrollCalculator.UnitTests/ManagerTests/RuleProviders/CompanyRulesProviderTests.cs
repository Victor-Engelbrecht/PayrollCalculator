using NSubstitute;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.RuleProviders;
using PayrollCalculator.Repositories.Contracts;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.ManagerTests.RuleProviders;

[TestFixture]
public class CompanyRulesProviderTests
{
    private ICompanyPayrollConfigRepository _configRepo = null!;
    private CompanyRulesProvider            _provider   = null!;

    [SetUp]
    public void SetUp()
    {
        _configRepo = Substitute.For<ICompanyPayrollConfigRepository>();
        _provider   = new CompanyRulesProvider(_configRepo);
    }

    [Test]
    public async Task Given_CompanyWithNoConfig_When_GetRulesAsync_Then_EmptyRulesAreReturned()
    {
        // Given
        _configRepo.GetAsync(Arg.Any<int>()).Returns((CompanyPayrollConfig?)null);
        var context = new PayrollRuleContextBuilder().Build();

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules, Is.Empty);
    }

    [Test]
    public async Task Given_CompanyWithConfig_When_GetRulesAsync_Then_SingleRuleIsReturned()
    {
        // Given
        var config  = new CompanyPayrollConfigBuilder().Build();
        var context = new PayrollRuleContextBuilder().Build();
        _configRepo.GetAsync(context.Company.Id).Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task Given_CompanyWithConfig_When_GetRulesAsync_Then_ReturnedRuleIsFlatTaxRule()
    {
        // Given
        var config  = new CompanyPayrollConfigBuilder().Build();
        var context = new PayrollRuleContextBuilder().Build();
        _configRepo.GetAsync(context.Company.Id).Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Single(), Is.InstanceOf<FlatTaxRule>());
    }

    [Test]
    public async Task Given_CompanyWithConfig_When_GetRulesAsync_Then_FlatTaxRuleUsesConfiguredTaxRate()
    {
        // Given
        var config  = new CompanyPayrollConfigBuilder().WithTaxRate(0.30m).Build();
        var context = new PayrollRuleContextBuilder().Build();
        var calcCtx = new PayCalculationContextBuilder().WithGrossPay(10000m).Build();
        _configRepo.GetAsync(context.Company.Id).Returns(config);

        // When
        var rules = await _provider.GetRulesAsync(context);
        rules.Single().Apply(calcCtx, []);

        // Then
        Assert.That(calcCtx.TotalDeductions, Is.EqualTo(3000m));
    }
}
