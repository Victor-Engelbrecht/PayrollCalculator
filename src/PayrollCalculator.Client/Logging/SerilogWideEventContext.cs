using PayrollCalculator.Utilities.Contracts;
using Serilog;

namespace PayrollCalculator.Client.Logging;

public sealed class SerilogWideEventContext : IWideEventContext
{
    private readonly IDiagnosticContext _diagnosticContext;

    public SerilogWideEventContext(IDiagnosticContext diagnosticContext)
    {
        _diagnosticContext = diagnosticContext;
    }

    public void Set(string propertyName, object? value, bool destructureObjects = false)
    {
        _diagnosticContext.Set(propertyName, value, destructureObjects);
    }
}
