using PayrollCalculator.Domain.Models;
using PayrollCalculator.Engines;
using PayrollCalculator.Engines.Rules;

namespace PayrollCalculator.UnitTests.EngineTests;

[TestFixture]
public class PayCalculationEngineTests
{
    private PayCalculationEngine _engine = null!;

    [SetUp]
    public void SetUp() => _engine = new PayCalculationEngine();

    private static IEnumerable<IPayrollRule> DefaultRules(decimal baseSalary) =>
        [new BaseSalaryRule(baseSalary), new FlatTaxRule(0.20m), new MinimumWageRule(1500m)];

    [Test]
    public void Given_StandardRules_When_Calculated_Then_NetAmountIsCorrect()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.NetAmount, Is.EqualTo(4000m));
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_TotalAdditionsIsCorrect()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.TotalAdditions, Is.EqualTo(5000m));
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_TotalDeductionsIsCorrect()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.TotalDeductions, Is.EqualTo(1000m));
    }

    [Test]
    public void Given_NetPayAboveMinimum_When_Calculated_Then_NoViolationsReturned()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Given_NetPayBelowMinimum_When_Calculated_Then_MinimumWageViolationReturned()
    {
        // Given
        var rules = DefaultRules(1800m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.Violations, Has.Count.EqualTo(1));
        Assert.That(result.Violations[0].RuleName, Is.EqualTo(nameof(MinimumWageRule)));
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_AdditionsAreAppliedBeforeDeductions()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.TotalDeductions, Is.GreaterThan(0m));
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_DeductionsAreAppliedBeforeCompliance()
    {
        // Given
        var rules = DefaultRules(1800m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.Violations, Is.Not.Empty);
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_TwoLineItemsReturned()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.LineItems, Has.Count.EqualTo(2));
    }

    [Test]
    public void Given_StandardRules_When_Calculated_Then_LineItemsContainBothAdditionAndDeductionKinds()
    {
        // Given
        var rules = DefaultRules(5000m);

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.LineItems.Any(li => li.Kind == PayslipLineItemKind.Addition),  Is.True);
        Assert.That(result.LineItems.Any(li => li.Kind == PayslipLineItemKind.Deduction), Is.True);
    }

    [Test]
    public void Given_NoRules_When_Calculated_Then_AllAmountsAreZeroAndNoViolations()
    {
        // Given
        var rules = Array.Empty<IPayrollRule>();

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.NetAmount,       Is.EqualTo(0m));
        Assert.That(result.TotalAdditions,  Is.EqualTo(0m));
        Assert.That(result.TotalDeductions, Is.EqualTo(0m));
        Assert.That(result.Violations,      Is.Empty);
    }

    [Test]
    public void Given_OnlyComplianceRule_When_Calculated_Then_NoLineItemsReturned()
    {
        // Given
        var rules = new IPayrollRule[] { new MinimumWageRule(1500m) };

        // When
        var result = _engine.Calculate(rules);

        // Then
        Assert.That(result.LineItems, Is.Empty);
    }
}
