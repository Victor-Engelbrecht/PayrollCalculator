using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class FlatTaxRuleTests
{
    private static PayCalculationContext MakeContext(decimal grossPay) => new()
    {
        Employee       = new Employee(),
        TotalAdditions = grossPay
    };

    [Test]
    public void Apply_DeductsTaxFromGrossPay()
    {
        var rule    = new FlatTaxRule(0.20m);
        var context = MakeContext(5000m);

        rule.Apply(context, []);

        Assert.That(context.TotalDeductions, Is.EqualTo(1000m));
    }

    [Test]
    public void Apply_RoundsDeductionToTwoDecimalPlaces()
    {
        var rule    = new FlatTaxRule(0.20m);
        var context = MakeContext(3333.33m); // 3333.33 * 0.20 = 666.666 -> 666.67

        rule.Apply(context, []);

        Assert.That(context.TotalDeductions, Is.EqualTo(666.67m));
    }

    [Test]
    public void Apply_WithZeroGross_DeductsZero()
    {
        var rule    = new FlatTaxRule(0.20m);
        var context = MakeContext(0m);

        rule.Apply(context, []);

        Assert.That(context.TotalDeductions, Is.EqualTo(0m));
    }

    [Test]
    public void Apply_AddsLineItemWithCorrectRuleName()
    {
        var rule    = new FlatTaxRule(0.20m);
        var context = MakeContext(5000m);

        rule.Apply(context, []);

        Assert.That(context.LineItems, Has.Count.EqualTo(1));
        Assert.That(context.LineItems[0].RuleName, Is.EqualTo(nameof(FlatTaxRule)));
    }

    [Test]
    public void Apply_AddsLineItemWithCorrectAmountAndKind()
    {
        var rule    = new FlatTaxRule(0.20m);
        var context = MakeContext(5000m);

        rule.Apply(context, []);

        var item = context.LineItems[0];
        Assert.That(item.Amount, Is.EqualTo(1000m));
        Assert.That(item.Kind,   Is.EqualTo(PayslipLineItemKind.Deduction));
    }

    [Test]
    public void Apply_DoesNotAddViolations()
    {
        var rule       = new FlatTaxRule(0.20m);
        var context    = MakeContext(5000m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Effect_IsDeduction()
    {
        var rule = new FlatTaxRule(0.20m);

        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Deduction));
    }
}
