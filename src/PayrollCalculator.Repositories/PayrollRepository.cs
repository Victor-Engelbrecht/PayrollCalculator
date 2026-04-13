using Dapper;
using PayrollCalculator.Domain.Models;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PayrollRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    // --- Payroll ---

    public async Task<Payroll?> GetByIdAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryFirstOrDefaultAsync<Payroll>(
            "SELECT * FROM Payrolls WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<Payroll>> GetByCompanyIdAsync(int companyId)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<Payroll>(
            "SELECT * FROM Payrolls WHERE CompanyId = @CompanyId", new { CompanyId = companyId });
    }

    public async Task<int> CreateAsync(Payroll payroll)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Payrolls
                (CompanyId, PeriodStart, PeriodEnd, RunAt, Status, CreatedAt, UpdatedAt)
            VALUES
                (@CompanyId, @PeriodStart, @PeriodEnd, @RunAt, @Status, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", payroll);
    }

    public async Task UpdateStatusAsync(int payrollId, string status)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync(@"
            UPDATE Payrolls
            SET Status    = @Status,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id",
            new { Id = payrollId, Status = status, UpdatedAt = DateTime.UtcNow });
    }

    // --- Payslips ---

    public async Task<PayslipDetail?> GetPayslipByIdAsync(int id)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryFirstOrDefaultAsync<PayslipDetail>(
            "SELECT * FROM Payslips WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<PayslipDetail>> GetPayslipsByPayrollIdAsync(int payrollId)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<PayslipDetail>(
            "SELECT * FROM Payslips WHERE PayrollId = @PayrollId", new { PayrollId = payrollId });
    }

    public async Task<int> CreatePayslipAsync(PayslipDetail payslip)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Payslips
                (PayrollId, EmployeeId, BaseSalary, TotalAdditions, TotalDeductions,
                 NetAmount, PaymentReference, CreatedAt, UpdatedAt)
            VALUES
                (@PayrollId, @EmployeeId, @BaseSalary, @TotalAdditions, @TotalDeductions,
                 @NetAmount, @PaymentReference, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", payslip);
    }

    public async Task UpdatePayslipPaymentReferenceAsync(int payslipId, string paymentReference)
    {
        using var c = _connectionFactory.CreateConnection();
        await c.ExecuteAsync(@"
            UPDATE Payslips
            SET PaymentReference = @PaymentReference,
                UpdatedAt        = @UpdatedAt
            WHERE Id = @Id",
            new { Id = payslipId, PaymentReference = paymentReference, UpdatedAt = DateTime.UtcNow });
    }

    // --- Additions ---

    public async Task<int> CreateAdditionAsync(Addition addition)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Additions
                (EmployeeId, PayrollId, Description, Amount, CreatedAt, UpdatedAt)
            VALUES
                (@EmployeeId, @PayrollId, @Description, @Amount, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", addition);
    }

    public async Task<IEnumerable<Addition>> GetAdditionsByPayslipAsync(int payrollId, int employeeId)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<Addition>(
            "SELECT * FROM Additions WHERE PayrollId = @PayrollId AND EmployeeId = @EmployeeId",
            new { PayrollId = payrollId, EmployeeId = employeeId });
    }

    // --- Deductions ---

    public async Task<int> CreateDeductionAsync(Deduction deduction)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QuerySingleAsync<int>(@"
            INSERT INTO Deductions
                (EmployeeId, PayrollId, Description, Amount, CreatedAt, UpdatedAt)
            VALUES
                (@EmployeeId, @PayrollId, @Description, @Amount, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();", deduction);
    }

    public async Task<IEnumerable<Deduction>> GetDeductionsByPayslipAsync(int payrollId, int employeeId)
    {
        using var c = _connectionFactory.CreateConnection();
        return await c.QueryAsync<Deduction>(
            "SELECT * FROM Deductions WHERE PayrollId = @PayrollId AND EmployeeId = @EmployeeId",
            new { PayrollId = payrollId, EmployeeId = employeeId });
    }
}
