namespace PayrollCalculator.Domain.Models.Adapters;

public record PaymentResult
{
    public bool Success { get; set; }
    public string? ProviderReference { get; set; }
    public string? ErrorMessage { get; set; }
}
