namespace PayrollCalculator.Utilities.Contracts;

public interface IWideEventContext
{
    void Set(string propertyName, object? value, bool destructureObjects = false);
}
