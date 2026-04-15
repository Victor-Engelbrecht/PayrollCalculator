IF OBJECT_ID('Companies', 'U') IS NULL
CREATE TABLE Companies (
    Id                  INT            IDENTITY(1,1) PRIMARY KEY,
    Name                NVARCHAR(255)  NOT NULL,
    RegistrationNumber  NVARCHAR(255)  NOT NULL,
    ContactEmail        NVARCHAR(255)  NULL,
    CreatedAt           DATETIME2      NOT NULL,
    UpdatedAt           DATETIME2      NOT NULL
);
