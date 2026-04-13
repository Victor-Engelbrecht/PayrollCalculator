using PayrollCalculator.Adapters.Contracts;
using PayrollCalculator.Domain.Models.Adapters;

namespace PayrollCalculator.Adapters;

public class PaymentProviderAdapter : IPaymentAdapter
{
    public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request) => throw new NotImplementedException();
}
