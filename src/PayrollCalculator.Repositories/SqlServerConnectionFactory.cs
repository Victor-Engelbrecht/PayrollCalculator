using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class SqlServerConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("PayrollDb")
            ?? throw new InvalidOperationException("Connection string 'PayrollDb' is not configured.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
