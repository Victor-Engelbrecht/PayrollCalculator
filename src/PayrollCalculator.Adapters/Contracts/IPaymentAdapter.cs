using PayrollCalculator.Domain.Models.Adapters;

namespace PayrollCalculator.Adapters.Contracts;

public interface IPaymentAdapter
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
}
