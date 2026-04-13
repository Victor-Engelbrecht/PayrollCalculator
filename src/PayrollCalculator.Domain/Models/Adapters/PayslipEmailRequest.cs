using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.Domain.Models.Adapters;

public record PayslipEmailRequest
{
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public PayslipDetail Payslip { get; set; } = new();
}
