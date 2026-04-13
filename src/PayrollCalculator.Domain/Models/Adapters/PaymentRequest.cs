namespace PayrollCalculator.Domain.Models.Adapters;

public record PaymentRequest
{
    public int PayslipDetailId { get; set; }
    public string RecipientBankAccount { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
}
