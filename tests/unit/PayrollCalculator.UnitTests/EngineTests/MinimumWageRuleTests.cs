using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class MinimumWageRuleTests
{
    private static PayCalculationContext MakeContext(decimal netPay) => new()
    {
        Employee       = new Employee(),
        TotalAdditions = netPay  // no deductions, so NetPay == netPay
    };

    [Test]
    public void Apply_WhenNetPayBelowMinimum_AddsViolation()
    {
        var rule       = new MinimumWageRule(1500m);
        var context    = MakeContext(1000m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations, Has.Count.EqualTo(1));
    }

    [Test]
    public void Apply_WhenNetPayBelowMinimum_ViolationHasCorrectRuleName()
    {
        var rule       = new MinimumWageRule(1500m);
        var context    = MakeContext(1000m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations[0].RuleName, Is.EqualTo(nameof(MinimumWageRule)));
    }

    [Test]
    public void Apply_WhenNetPayAtMinimum_NoViolation()
    {
        var rule       = new MinimumWageRule(1500m);
        var context    = MakeContext(1500m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Apply_WhenNetPayAboveMinimum_NoViolation()
    {
        var rule       = new MinimumWageRule(1500m);
        var context    = MakeContext(4000m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Apply_DoesNotModifyContextAmounts()
    {
        var rule    = new MinimumWageRule(1500m);
        var context = MakeContext(1000m);

        rule.Apply(context, []);

        Assert.That(context.TotalAdditions,  Is.EqualTo(1000m));
        Assert.That(context.TotalDeductions, Is.EqualTo(0m));
    }

    [Test]
    public void Apply_DoesNotAddLineItems()
    {
        var rule    = new MinimumWageRule(1500m);
        var context = MakeContext(1000m);

        rule.Apply(context, []);

        Assert.That(context.LineItems, Is.Empty);
    }

    [Test]
    public void Effect_IsCompliance()
    {
        var rule = new MinimumWageRule(1500m);

        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Compliance));
    }
}
