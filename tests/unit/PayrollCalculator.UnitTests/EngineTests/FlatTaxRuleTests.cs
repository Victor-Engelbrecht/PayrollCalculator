using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;
using PayrollCalculator.UnitTests.Builders;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class FlatTaxRuleTests
{
    [Test]
    public void Given_FlatTaxRule_When_Applied_Then_TaxIsDeductedFromGrossPay()
    {
        // Given
        var rule    = new FlatTaxRule(0.20m);
        var context = new PayCalculationContextBuilder().WithGrossPay(5000m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalDeductions, Is.EqualTo(1000m));
    }

    [Test]
    public void Given_FlatTaxRuleWithFractionalResult_When_Applied_Then_DeductionIsRoundedToTwoDecimalPlaces()
    {
        // Given
        var rule    = new FlatTaxRule(0.20m);
        var context = new PayCalculationContextBuilder().WithGrossPay(3333.33m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalDeductions, Is.EqualTo(666.67m));
    }

    [Test]
    public void Given_FlatTaxRuleWithZeroGrossPay_When_Applied_Then_TotalDeductionsIsZero()
    {
        // Given
        var rule    = new FlatTaxRule(0.20m);
        var context = new PayCalculationContextBuilder().WithGrossPay(0m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.TotalDeductions, Is.EqualTo(0m));
    }

    [Test]
    public void Given_FlatTaxRule_When_Applied_Then_LineItemHasCorrectRuleName()
    {
        // Given
        var rule    = new FlatTaxRule(0.20m);
        var context = new PayCalculationContextBuilder().WithGrossPay(5000m).Build();

        // When
        rule.Apply(context, []);

        // Then
        Assert.That(context.LineItems, Has.Count.EqualTo(1));
        Assert.That(context.LineItems[0].RuleName, Is.EqualTo(nameof(FlatTaxRule)));
    }

    [Test]
    public void Given_FlatTaxRule_When_Applied_Then_LineItemHasCorrectAmountAndKind()
    {
        // Given
        var rule    = new FlatTaxRule(0.20m);
        var context = new PayCalculationContextBuilder().WithGrossPay(5000m).Build();

        // When
        rule.Apply(context, []);

        // Then
        var item = context.LineItems[0];
        Assert.That(item.Amount, Is.EqualTo(1000m));
        Assert.That(item.Kind,   Is.EqualTo(PayslipLineItemKind.Deduction));
    }

    [Test]
    public void Given_FlatTaxRule_When_Applied_Then_NoViolationsAreAdded()
    {
        // Given
        var rule       = new FlatTaxRule(0.20m);
        var context    = new PayCalculationContextBuilder().WithGrossPay(5000m).Build();
        var violations = new List<RuleViolation>();

        // When
        rule.Apply(context, violations);

        // Then
        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Given_FlatTaxRule_When_EffectIsRead_Then_EffectIsDeduction()
    {
        // Given
        var rule = new FlatTaxRule(0.20m);

        // When / Then
        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Deduction));
    }
}
