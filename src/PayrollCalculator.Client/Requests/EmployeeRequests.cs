namespace PayrollCalculator.Client.Requests;

public record CreateEmployeeRequest(
    int CompanyId,
    string FirstName,
    string LastName,
    string Email,
    decimal BaseSalary,
    string? BankAccountNumber);

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    decimal BaseSalary,
    string? BankAccountNumber);

public record AddAdditionRequest(
    string Description,
    decimal Amount);

public record AddDeductionRequest(
    string Description,
    decimal Amount);
