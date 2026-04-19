namespace PayrollCalculator.Domain.Models;

public record CompanyPayrollConfig
{
    public int     CompanyId { get; init; }
    public decimal TaxRate   { get; init; }
}
