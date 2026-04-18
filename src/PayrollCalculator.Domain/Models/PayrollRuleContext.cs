namespace PayrollCalculator.Domain.Models;

public record PayrollRuleContext
{
    public Company Company { get; init; } = null!;
    public Employee Employee { get; init; } = null!;
}
