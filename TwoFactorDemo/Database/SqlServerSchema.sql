IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        Username NVARCHAR(100) NOT NULL,
        PasswordSalt VARBINARY(16) NOT NULL,
        PasswordHash VARBINARY(32) NOT NULL,
        TotpSecret NVARCHAR(128) NOT NULL,
        CreatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAtUtc DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Users_UpdatedAtUtc DEFAULT SYSUTCDATETIME(),
        TotpSecretUpdatedAtUtc DATETIME2 NOT NULL CONSTRAINT DF_Users_TotpSecretUpdatedAtUtc DEFAULT SYSUTCDATETIME()
    );

    CREATE UNIQUE INDEX UX_Users_Username ON dbo.Users (Username);
END
GO
