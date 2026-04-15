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
);
