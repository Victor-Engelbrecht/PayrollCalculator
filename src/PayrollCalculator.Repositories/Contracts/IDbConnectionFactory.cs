using System.Data;

namespace PayrollCalculator.Repositories.Contracts;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
