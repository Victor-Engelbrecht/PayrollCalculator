using Dapper;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public EmployeeRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Employee?> GetByIdAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryFirstOrDefaultAsync<Employee>(
            "SELECT * FROM Employees WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<Employee>(
            "SELECT * FROM Employees WHERE CompanyId = @CompanyId", new { CompanyId = companyId });
    }

    public async Task<int> CreateAsync(Employee employee)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Employees
                (CompanyId, FirstName, LastName, Email, BaseSalary, BankAccountNumber, CreatedAt, UpdatedAt)
            VALUES
                (@CompanyId, @FirstName, @LastName, @Email, @BaseSalary, @BankAccountNumber, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", employee);
    }

    public async Task UpdateAsync(Employee employee)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync(@"
            UPDATE Employees
            SET FirstName         = @FirstName,
                LastName          = @LastName,
                Email             = @Email,
                BaseSalary        = @BaseSalary,
                BankAccountNumber = @BankAccountNumber,
                UpdatedAt         = @UpdatedAt
            WHERE Id = @Id", employee);
    }

    public async Task DeleteAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync("DELETE FROM Employees WHERE Id = @Id", new { Id = id });
    }
}
