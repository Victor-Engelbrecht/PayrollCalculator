namespace PayrollCalculator.Domain.Models;

public record PayCalculationResult
{
    public decimal NetAmount { get; set; }
    public decimal TotalAdditions { get; set; }
    public decimal TotalDeductions { get; set; }
    public IReadOnlyList<RuleViolation>   Violations { get; set; } = [];
    public IReadOnlyList<PayslipLineItem> LineItems  { get; set; } = [];
}
