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
);
