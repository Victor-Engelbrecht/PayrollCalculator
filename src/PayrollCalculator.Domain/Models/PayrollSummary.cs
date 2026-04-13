namespace PayrollCalculator.Domain.Models;

public record PayrollSummary
{
    public int PayrollId { get; set; }
    public int EmployeeCount { get; set; }
    public decimal TotalNetPaid { get; set; }
    public List<RuleViolation> Violations { get; set; } = [];
    public DateTime CompletedAt { get; set; }
}
