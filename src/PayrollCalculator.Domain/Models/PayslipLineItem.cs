namespace PayrollCalculator.Domain.Models;

public record PayslipLineItem
{
    public string RuleName    { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Amount     { get; init; }
    public PayslipLineItemKind Kind { get; init; }
}

public enum PayslipLineItemKind { Addition, Deduction }
