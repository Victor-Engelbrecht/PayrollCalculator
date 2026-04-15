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
);
