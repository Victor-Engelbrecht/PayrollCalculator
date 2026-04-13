namespace PayrollCalculator.Client.Responses;

public record PayrollResponse(
    int Id,
    int CompanyId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    DateTime RunAt,
    string Status);

public record PayslipResponse(
    int Id,
    int PayrollId,
    int EmployeeId,
    decimal BaseSalary,
    decimal TotalAdditions,
    decimal TotalDeductions,
    decimal NetAmount,
    string? PaymentReference);

public record RuleViolationResponse(
    string RuleName,
    string Message);

public record PayrollSummaryResponse(
    int PayrollId,
    int EmployeeCount,
    decimal TotalNetPaid,
    List<RuleViolationResponse> Violations,
    DateTime CompletedAt);
