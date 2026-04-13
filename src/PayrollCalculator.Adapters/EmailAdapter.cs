using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models.Adapters;

namespace PayrollCalculator.Adapters;

public class EmailAdapter : IEmailAdapter
{
    public Task SendPayslipAsync(PayslipEmailRequest request) => throw new NotImplementedException();
}
