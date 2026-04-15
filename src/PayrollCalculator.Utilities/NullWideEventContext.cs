using PayrollCalculator.Utilities.Contracts;

namespace PayrollCalculator.Utilities;

public sealed class NullWideEventContext : IWideEventContext
{
    public static readonly NullWideEventContext Instance = new();

    public void Set(string propertyName, object? value, bool destructureObjects = false)
    {
    }
}
