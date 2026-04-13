namespace PayrollCalculator.Client.Requests;

public record RunPayrollRequest(
    int CompanyId,
    DateTime PeriodStart,
    DateTime PeriodEnd);
