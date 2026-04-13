using PayrollCalculator.Domain.Models.Adapters;

namespace PayrollCalculator.Adapters.Contracts;

public interface IEmailAdapter
{
    Task SendPayslipAsync(PayslipEmailRequest request);
}
