using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PayrollCalculator.Managers.Contracts;

namespace PayrollCalculator.Client.Workers;

public class PayrollWorker : BackgroundService
{
    private readonly IPayrollManager _payrollManager;
    private readonly ILogger<PayrollWorker> _logger;

    public PayrollWorker(IPayrollManager payrollManager, ILogger<PayrollWorker> logger)
    {
        _payrollManager = payrollManager;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => throw new NotImplementedException();
}
