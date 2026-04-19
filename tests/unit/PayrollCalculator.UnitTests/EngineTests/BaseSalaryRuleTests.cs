using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class BaseSalaryRuleTests
{
    [Test]
    public void Given_BaseSalaryRule_When_Applied_Then_SalaryIsAddedToTotalAdditions()
    {
        // Given
        var rule    = new BaseSalaryRule(5000m);
        var context = new PayCalculationContextBuilder().Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalAdditions, Is.EqualTo(5000m));
    }

    [Test]
    public void Given_BaseSalaryRuleWithZeroSalary_When_Applied_Then_TotalAdditionsIsZero()
    {
        // Given
        var rule    = new BaseSalaryRule(0m);
        var context = new PayCalculationContextBuilder().Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalAdditions, Is.EqualTo(0m));
    }

    [Test]
    public void Given_BaseSalaryRule_When_Applied_Then_LineItemHasCorrectRuleName()
    {
        // Given
        var rule    = new BaseSalaryRule(3000m);
        var context = new PayCalculationContextBuilder().Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.LineItems, Has.Count.EqualTo(1));
        Assert.That(context.LineItems[0].RuleName, Is.EqualTo(nameof(BaseSalaryRule)));
    }

    [Test]
    public void Given_BaseSalaryRule_When_Applied_Then_LineItemHasCorrectAmountAndKind()
    {
        // Given
        var rule    = new BaseSalaryRule(3000m);
        var context = new PayCalculationContextBuilder().Build();

        // When
        rule.Apply(context, []);

        // Then
        var item = context.LineItems[0];
        Assert.That(item.Amount, Is.EqualTo(3000m));
        Assert.That(item.Kind,   Is.EqualTo(PayslipLineItemKind.Addition));
    }

    [Test]
    public void Given_BaseSalaryRule_When_Applied_Then_NoViolationsAreAdded()
    {
        // Given
        var rule       = new BaseSalaryRule(5000m);
        var context    = new PayCalculationContextBuilder().Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Given_BaseSalaryRule_When_EffectIsRead_Then_EffectIsAddition()
    {
        // Given
        var rule = new BaseSalaryRule(0m);

        // When / Then
        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Addition));
    }
}
