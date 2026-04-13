namespace PayrollCalculator.Domain.Models;

public record PayCalculationContext
{
    public Employee Employee { get; set; } = null!;
    public List<Addition> Additions { get; set; } = [];
    public List<Deduction> Deductions { get; set; } = [];
    public decimal GrossPay { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
}
