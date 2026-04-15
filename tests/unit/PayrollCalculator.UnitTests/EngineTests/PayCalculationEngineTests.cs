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

    private static Employee MakeEmployee(decimal baseSalary) => new()
    {
        Id        = 1,
        CompanyId = 1,
        FirstName = "Test",
        LastName  = "Employee",
        Email     = "test@example.com",
        BaseSalary = baseSalary
    };

    private static IEnumerable<IPayrollRule> DefaultRules() =>
        [new BaseSalaryRule(), new FlatTaxRule(0.20m), new MinimumWageRule(1500m)];

    [Test]
    public void Calculate_ReturnsCorrectNetAmount()
    {
        // Gross = 5000, Tax = 1000, Net = 4000
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.NetAmount, Is.EqualTo(4000m));
    }

    [Test]
    public void Calculate_ReturnsCorrectTotalAdditions()
    {
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.TotalAdditions, Is.EqualTo(5000m));
    }

    [Test]
    public void Calculate_ReturnsCorrectTotalDeductions()
    {
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.TotalDeductions, Is.EqualTo(1000m));
    }

    [Test]
    public void Calculate_WhenNetPayAboveMinimum_ReturnsNoViolations()
    {
        // Net = 5000 * 0.80 = 4000 >= 1500
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.Violations, Is.Empty);
    }

    [Test]
    public void Calculate_WhenNetPayBelowMinimum_ReturnsViolation()
    {
        // Net = 1800 * 0.80 = 1440 < 1500
        var result = _engine.Calculate(MakeEmployee(1800m), DefaultRules());

        Assert.That(result.Violations, Has.Count.EqualTo(1));
        Assert.That(result.Violations[0].RuleName, Is.EqualTo(nameof(MinimumWageRule)));
    }

    [Test]
    public void Calculate_AppliesAdditionsBeforeDeductions()
    {
        // If deductions ran first, GrossPay would be 0 and tax would be 0 instead of 1000.
        // A non-zero TotalDeductions proves additions ran first.
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.TotalDeductions, Is.GreaterThan(0m));
    }

    [Test]
    public void Calculate_AppliesDeductionsBeforeCompliance()
    {
        // With salary 1800: Net after deductions = 1440 (below 1500 -> violation).
        // If compliance ran before deductions, NetPay would still be 1800 (no violation).
        var result = _engine.Calculate(MakeEmployee(1800m), DefaultRules());

        Assert.That(result.Violations, Is.Not.Empty,
            "Compliance rule should see the post-deduction net pay");
    }

    [Test]
    public void Calculate_ReturnsLineItemsForEachRule()
    {
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        // BaseSalaryRule + FlatTaxRule each produce a line item; MinimumWageRule does not
        Assert.That(result.LineItems, Has.Count.EqualTo(2));
    }

    [Test]
    public void Calculate_LineItemsIncludeAdditionAndDeductionKinds()
    {
        var result = _engine.Calculate(MakeEmployee(5000m), DefaultRules());

        Assert.That(result.LineItems.Any(li => li.Kind == PayslipLineItemKind.Addition),  Is.True);
        Assert.That(result.LineItems.Any(li => li.Kind == PayslipLineItemKind.Deduction), Is.True);
    }

    [Test]
    public void Calculate_WithNoRules_ReturnsZeroAmounts()
    {
        var result = _engine.Calculate(MakeEmployee(5000m), []);

        Assert.That(result.NetAmount,       Is.EqualTo(0m));
        Assert.That(result.TotalAdditions,  Is.EqualTo(0m));
        Assert.That(result.TotalDeductions, Is.EqualTo(0m));
        Assert.That(result.Violations,      Is.Empty);
    }
}
