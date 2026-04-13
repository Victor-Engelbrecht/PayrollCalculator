namespace PayrollCalculator.Client.Responses;

public record CompanyResponse(
    int Id,
    string Name,
    string RegistrationNumber,
    string? ContactEmail,
    DateTime CreatedAt,
    DateTime UpdatedAt);
