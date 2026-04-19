using PayrollCalculator.Domain.Models;

namespace PayrollCalculator.UnitTests.Builders;

public class PayrollBuilder
{
    private int    _id        = 1;
    private int    _companyId = 1;
    private string _status    = "Running";

    public PayrollBuilder WithId(int id)            { _id        = id;        return this; }
    public PayrollBuilder WithCompanyId(int id)     { _companyId = id;        return this; }
    public PayrollBuilder WithStatus(string status) { _status    = status;    return this; }

    public Payroll Build() => new()
    {
        Id         = _id,
        CompanyId  = _companyId,
        PeriodStart = DateTime.UtcNow,
        PeriodEnd   = DateTime.UtcNow,
        RunAt       = DateTime.UtcNow,
        Status     = _status
    };
}
