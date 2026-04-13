using Dapper;
using PayrollCalculator.Repositories.Contracts;

namespace PayrollCalculator.Repositories;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Companies', 'U') IS NULL
            CREATE TABLE Companies (
                Id                  INT            IDENTITY(1,1) PRIMARY KEY,
                Name                NVARCHAR(255)  NOT NULL,
                RegistrationNumber  NVARCHAR(255)  NOT NULL,
                ContactEmail        NVARCHAR(255)  NULL,
                CreatedAt           DATETIME2      NOT NULL,
                UpdatedAt           DATETIME2      NOT NULL
            );");

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Employees', 'U') IS NULL
            CREATE TABLE Employees (
                Id                  INT            IDENTITY(1,1) PRIMARY KEY,
                CompanyId           INT            NOT NULL,
                FirstName           NVARCHAR(255)  NOT NULL,
                LastName            NVARCHAR(255)  NOT NULL,
                Email               NVARCHAR(255)  NOT NULL,
                BaseSalary          DECIMAL(18,4)  NOT NULL,
                BankAccountNumber   NVARCHAR(255)  NULL,
                CreatedAt           DATETIME2      NOT NULL,
                UpdatedAt           DATETIME2      NOT NULL,
                FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
            );");

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Payrolls', 'U') IS NULL
            CREATE TABLE Payrolls (
                Id          INT            IDENTITY(1,1) PRIMARY KEY,
                CompanyId   INT            NOT NULL,
                PeriodStart DATETIME2      NOT NULL,
                PeriodEnd   DATETIME2      NOT NULL,
                RunAt       DATETIME2      NOT NULL,
                Status      NVARCHAR(50)   NOT NULL,
                CreatedAt   DATETIME2      NOT NULL,
                UpdatedAt   DATETIME2      NOT NULL,
                FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
            );");

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Payslips', 'U') IS NULL
            CREATE TABLE Payslips (
                Id               INT            IDENTITY(1,1) PRIMARY KEY,
                PayrollId        INT            NOT NULL,
                EmployeeId       INT            NOT NULL,
                BaseSalary       DECIMAL(18,4)  NOT NULL,
                TotalAdditions   DECIMAL(18,4)  NOT NULL,
                TotalDeductions  DECIMAL(18,4)  NOT NULL,
                NetAmount        DECIMAL(18,4)  NOT NULL,
                PaymentReference NVARCHAR(255)  NULL,
                CreatedAt        DATETIME2      NOT NULL,
                UpdatedAt        DATETIME2      NOT NULL,
                FOREIGN KEY (PayrollId)  REFERENCES Payrolls(Id),
                FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
            );");

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Additions', 'U') IS NULL
            CREATE TABLE Additions (
                Id          INT            IDENTITY(1,1) PRIMARY KEY,
                EmployeeId  INT            NOT NULL,
                PayrollId   INT            NOT NULL,
                Description NVARCHAR(MAX)  NOT NULL,
                Amount      DECIMAL(18,4)  NOT NULL,
                CreatedAt   DATETIME2      NOT NULL,
                UpdatedAt   DATETIME2      NOT NULL,
                FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
                FOREIGN KEY (PayrollId)  REFERENCES Payrolls(Id)
            );");

        await connection.ExecuteAsync(@"
            IF OBJECT_ID('Deductions', 'U') IS NULL
            CREATE TABLE Deductions (
                Id          INT            IDENTITY(1,1) PRIMARY KEY,
                EmployeeId  INT            NOT NULL,
                PayrollId   INT            NOT NULL,
                Description NVARCHAR(MAX)  NOT NULL,
                Amount      DECIMAL(18,4)  NOT NULL,
                CreatedAt   DATETIME2      NOT NULL,
                UpdatedAt   DATETIME2      NOT NULL,
                FOREIGN KEY (EmployeeId) REFERENCES Employees(Id),
                FOREIGN KEY (PayrollId)  REFERENCES Payrolls(Id)
            );");
    }
}
