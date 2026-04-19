namespace PayrollCalculator.Domain.Models;

public record CountryPayrollConfig
{
    public string  CountryCode { get; init; } = string.Empty;
    public decimal MinimumWage { get; init; }
}
