using Microsoft.Data.SqlClient;

namespace TwoFactorDemo;

public sealed class SqlServerUserStore : IUserStore
{
    private readonly string _connectionString;

    public SqlServerUserStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("SQL Server connection string is required.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public void EnsureSchema()
    {
        const string sql = """
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
                           """;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }

    public DemoUser? FindByUsername(string username)
    {
        const string sql = """
                           SELECT Username, PasswordSalt, PasswordHash, TotpSecret
                           FROM dbo.Users
                           WHERE Username = @username
                           """;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new DemoUser(
            reader.GetString(0),
            (byte[])reader[1],
            (byte[])reader[2],
            reader.GetString(3));
    }

    public void Create(DemoUser user)
    {
        const string sql = """
                           INSERT INTO dbo.Users (Username, PasswordSalt, PasswordHash, TotpSecret, UpdatedAtUtc, TotpSecretUpdatedAtUtc)
                           VALUES (@username, @passwordSalt, @passwordHash, @totpSecret, SYSUTCDATETIME(), SYSUTCDATETIME())
                           """;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.Add("@passwordSalt", System.Data.SqlDbType.VarBinary, 16).Value = user.PasswordSalt;
        command.Parameters.Add("@passwordHash", System.Data.SqlDbType.VarBinary, 32).Value = user.PasswordHash;
        command.Parameters.AddWithValue("@totpSecret", user.TotpSecret);

        command.ExecuteNonQuery();
    }

    public void UpdateTotpSecret(string username, string totpSecret)
    {
        const string sql = """
                           UPDATE dbo.Users
                           SET TotpSecret = @totpSecret,
                               TotpSecretUpdatedAtUtc = SYSUTCDATETIME(),
                               UpdatedAtUtc = SYSUTCDATETIME()
                           WHERE Username = @username
                           """;

        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username);
        command.Parameters.AddWithValue("@totpSecret", totpSecret);

        var affected = command.ExecuteNonQuery();
        if (affected == 0)
        {
            throw new InvalidOperationException("User not found.");
        }
    }
}
