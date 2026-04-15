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
);
