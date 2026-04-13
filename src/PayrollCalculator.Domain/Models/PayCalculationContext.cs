namespace PayrollCalculator.Domain.Models;

public record PayCalculationContext
{
    public Employee Employee       { get; init; } = null!;
    public decimal TotalAdditions  { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal GrossPay        => Employee.BaseSalary + TotalAdditions;
    public decimal NetPay          => GrossPay - TotalDeductions;
}
