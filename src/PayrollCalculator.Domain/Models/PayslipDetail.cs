namespace PayrollCalculator.Domain.Models;

public record PayslipDetail
{
    public int Id { get; set; }
    public int PayrollId { get; set; }
    public int EmployeeId { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal TotalAdditions { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetAmount { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
