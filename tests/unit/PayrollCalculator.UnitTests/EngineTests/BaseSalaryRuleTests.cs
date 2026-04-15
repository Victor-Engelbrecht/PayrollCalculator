using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class BaseSalaryRuleTests
{
    private static PayCalculationContext MakeContext(decimal baseSalary) => new()
    {
        Employee = new Employee { BaseSalary = baseSalary }
    };

    [Test]
    public void Apply_AddsSalaryToTotalAdditions()
    {
        var rule    = new BaseSalaryRule();
        var context = MakeContext(5000m);

        rule.Apply(context, []);

        Assert.That(context.TotalAdditions, Is.EqualTo(5000m));
    }

    [Test]
    public void Apply_WithZeroSalary_AddZeroToTotalAdditions()
    {
        var rule    = new BaseSalaryRule();
        var context = MakeContext(0m);

        rule.Apply(context, []);

        Assert.That(context.TotalAdditions, Is.EqualTo(0m));
    }

    [Test]
    public void Apply_AddsLineItemWithCorrectRuleName()
    {
        var rule    = new BaseSalaryRule();
        var context = MakeContext(3000m);

        rule.Apply(context, []);

        Assert.That(context.LineItems, Has.Count.EqualTo(1));
        Assert.That(context.LineItems[0].RuleName, Is.EqualTo(nameof(BaseSalaryRule)));
    }

    [Test]
    public void Apply_AddsLineItemWithCorrectAmountAndKind()
    {
        var rule    = new BaseSalaryRule();
        var context = MakeContext(3000m);

        rule.Apply(context, []);

        var item = context.LineItems[0];
        Assert.That(item.Amount, Is.EqualTo(3000m));
        Assert.That(item.Kind,   Is.EqualTo(PayslipLineItemKind.Addition));
    }

    [Test]
    public void Apply_DoesNotAddViolations()
    {
        var rule       = new BaseSalaryRule();
        var context    = MakeContext(5000m);
        var violations = new List<RuleViolation>();

        rule.Apply(context, violations);

        Assert.That(violations, Is.Empty);
    }

    [Test]
    public void Effect_IsAddition()
    {
        var rule = new BaseSalaryRule();

        Assert.That(rule.Effect, Is.EqualTo(PayrollRuleEffect.Addition));
    }
}
