namespace PayrollCalculator.Client.Requests;

public record CreateCompanyRequest(
    string Name,
    string RegistrationNumber,
    string? ContactEmail);

public record UpdateCompanyRequest(
    string Name,
    string RegistrationNumber,
    string? ContactEmail);
