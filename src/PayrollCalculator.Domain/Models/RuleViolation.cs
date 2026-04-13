namespace PayrollCalculator.Domain.Models;

public record RuleViolation
{
    public string RuleName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
