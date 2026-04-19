using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class MinimumWageRuleTests
{
    [Test]
    public void Given_NetPayBelowMinimum_When_Applied_Then_OneViolationIsAdded()
    {
        // Given
        var rule       = new MinimumWageRule(1500m);
        var context    = new PayCalculationContextBuilder().WithNetPay(1000m).Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations, Has.Count.EqualTo(1));
    }

    [Test]
    public void Given_NetPayBelowMinimum_When_Applied_Then_ViolationHasCorrectRuleName()
    {
        // Given
        var rule       = new MinimumWageRule(1500m);
        var context    = new PayCalculationContextBuilder().WithNetPay(1000m).Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations[0].RuleName, Is.EqualTo(nameof(MinimumWageRule)));
    }

    [Test]
    public void Given_NetPayAtMinimum_When_Applied_Then_NoViolationIsAdded()
    {
        // Given
        var rule       = new MinimumWageRule(1500m);
        var context    = new PayCalculationContextBuilder().WithNetPay(1500m).Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Given_NetPayAboveMinimum_When_Applied_Then_NoViolationIsAdded()
    {
        // Given
        var rule       = new MinimumWageRule(1500m);
        var context    = new PayCalculationContextBuilder().WithNetPay(4000m).Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Given_NetPayBelowMinimum_When_Applied_Then_ContextAmountsAreNotModified()
    {
        // Given
        var rule    = new MinimumWageRule(1500m);
        var context = new PayCalculationContextBuilder().WithNetPay(1000m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalAdditions,  Is.EqualTo(1000m));
        Assert.That(context.TotalDeductions, Is.EqualTo(0m));
    }

    [Test]
    public void Given_NetPayBelowMinimum_When_Applied_Then_NoLineItemsAreAdded()
    {
        // Given
        var rule    = new MinimumWageRule(1500m);
        var context = new PayCalculationContextBuilder().WithNetPay(1000m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.LineItems, Is.Empty);
    }

    [Test]
    public void Given_MinimumWageRule_When_EffectIsRead_Then_EffectIsCompliance()
    {
        // Given
        var rule = new MinimumWageRule(1500m);

        // When / Then
        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Compliance));
    }
}
