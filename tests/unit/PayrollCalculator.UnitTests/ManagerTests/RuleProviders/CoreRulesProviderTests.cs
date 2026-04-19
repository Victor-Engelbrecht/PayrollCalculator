using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.Managers.RuleProviders;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.ManagerTests.RuleProviders;

[TestFixture]
public class CoreRulesProviderTests
{
    private CoreRulesProvider _provider = null!;

    [SetUp]
    public void SetUp() => _provider = new CoreRulesProvider();

    [Test]
    public async Task Given_EmployeeWithBaseSalary_When_GetRulesAsync_Then_SingleRuleIsReturned()
    {
        // Given
        var context = new PayrollRuleContextBuilder().Build();

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task Given_EmployeeWithBaseSalary_When_GetRulesAsync_Then_ReturnedRuleIsBaseSalaryRule()
    {
        // Given
        var context = new PayrollRuleContextBuilder().Build();

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules.Single(), Is.InstanceOf<BaseSalaryRule>());
    }

    [Test]
    public async Task Given_EmployeeWithSpecificBaseSalary_When_GetRulesAsync_Then_BaseSalaryRuleHasThatSalary()
    {
        // Given
        var employee = new EmployeeBuilder().WithBaseSalary(7500m).Build();
        var context  = new PayrollRuleContextBuilder().WithEmployee(employee).Build();
        var calcCtx  = new PayCalculationContextBuilder().Build();

        // When
        var rules = await _provider.GetRulesAsync(context);
        rules.Single().Apply(calcCtx, []);

        // Then
        Assert.That(calcCtx.TotalAdditions, Is.EqualTo(7500m));
    }
}
