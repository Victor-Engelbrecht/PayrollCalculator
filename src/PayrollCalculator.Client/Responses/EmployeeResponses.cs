namespace PayrollCalculator.Client.Responses;

public record EmployeeResponse(
    int Id,
    int CompanyId,
    string FirstName,
    string LastName,
    string Email,
    decimal BaseSalary,
    string? BankAccountNumber,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record AdditionResponse(
    int Id,
    int EmployeeId,
    int PayrollId,
    string Description,
    decimal Amount);

public record DeductionResponse(
    int Id,
    int EmployeeId,
    int PayrollId,
    string Description,
    decimal Amount);
