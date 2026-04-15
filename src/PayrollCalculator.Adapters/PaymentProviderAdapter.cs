using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models.Adapters;

namespace PayrollCalculator.Adapters;

public class PaymentProviderAdapter : IPaymentAdapter
{
    public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        => Task.FromResult(new PaymentResult
        {
            Success = true,
            ProviderReference = $"REF-{request.PayslipDetailId}-{Guid.NewGuid():N}"
        });
}
