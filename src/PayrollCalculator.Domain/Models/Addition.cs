namespace PayrollCalculator.Domain.Models;

public record Addition
{
    public int Id               { get; set; }
    public int EmployeeId       { get; set; }
    public int PayrollId        { get; set; }
    public string Description   { get; set; } = string.Empty;
    public decimal Amount       { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }
}
