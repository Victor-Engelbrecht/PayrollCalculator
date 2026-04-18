namespace PayrollCalculator.Domain.Models;

public record PayslipNotification
{
    public string RecipientEmail { get; init; } = string.Empty;
    public string RecipientName  { get; init; } = string.Empty;
    public PayslipDetail Payslip { get; init; } = null!;
}
