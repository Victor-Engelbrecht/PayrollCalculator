using PayrollCalculator.Managers.RuleProviders;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.ManagerTests.RuleProviders;

[TestFixture]
public class EmployeeRulesProviderTests
{
    private EmployeeRulesProvider _provider = null!;

    [SetUp]
    public void SetUp() => _provider = new EmployeeRulesProvider();

    [Test]
    public async Task Given_AnyContext_When_GetRulesAsync_Then_EmptyRulesAreReturned()
    {
        // Given
        var context = new PayrollRuleContextBuilder().Build();

        // When
        var rules = await _provider.GetRulesAsync(context);

        // Then
        Assert.That(rules, Is.Empty);
    }
}
