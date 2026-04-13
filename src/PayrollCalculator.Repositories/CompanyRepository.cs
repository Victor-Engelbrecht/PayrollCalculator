using Dapper;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CompanyRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Company?> GetByIdAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryFirstOrDefaultAsync<Company>(
            "SELECT * FROM Companies WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<Company>("SELECT * FROM Companies");
    }

    public async Task<int> CreateAsync(Company company)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Companies (Name, RegistrationNumber, ContactEmail, CreatedAt, UpdatedAt)
            VALUES (@Name, @RegistrationNumber, @ContactEmail, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", company);
    }

    public async Task UpdateAsync(Company company)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync(@"
            UPDATE Companies
            SET Name               = @Name,
                RegistrationNumber = @RegistrationNumber,
                ContactEmail       = @ContactEmail,
                UpdatedAt          = @UpdatedAt
            WHERE Id = @Id", company);
    }

    public async Task DeleteAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync("DELETE FROM Companies WHERE Id = @Id", new { Id = id });
    }
}
