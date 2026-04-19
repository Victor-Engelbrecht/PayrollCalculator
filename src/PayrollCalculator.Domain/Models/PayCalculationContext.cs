namespace PayrollCalculator.Domain.Models;

public record PayCalculationContext
{
    public decimal TotalAdditions  { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossPay        => TotalAdditions;
    public decimal NetPay          => GrossPay - TotalDeductions;

    public List<PayslipLineItem> LineItems { get; } = [];
}
