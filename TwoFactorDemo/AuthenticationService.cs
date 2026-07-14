using System.Security.Cryptography;
using System.Text;

namespace TwoFactorDemo;

public sealed class AuthenticationService
{
    private readonly IUserStore _userStore;

    public AuthenticationService(IUserStore? userStore = null)
    {
        _userStore = userStore ?? new InMemoryUserStore();
    }

    public RegistrationResult RegisterUser(string username, string password)
    {
        var existingUser = _userStore.FindByUsername(username);
        if (existingUser is not null)
        {
            return new RegistrationResult(false, null);
        }

        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = HashPassword(password, salt);
        var secret = TotpAuthenticator.GenerateSecret();
        _userStore.Create(new DemoUser(username, salt, hash, secret));

        return new RegistrationResult(true, secret);
    }

    public void AddUser(string username, string password, string totpSecret)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = HashPassword(password, salt);
        _userStore.Create(new DemoUser(username, salt, hash, totpSecret));
    }

    public bool ValidatePassword(string username, string password)
    {
        var user = _userStore.FindByUsername(username);
        if (user is null)
        {
            return false;
        }

        var incomingHash = HashPassword(password, user.PasswordSalt);
        return CryptographicOperations.FixedTimeEquals(incomingHash, user.PasswordHash);
    }

    public bool ValidateTotp(string username, string code, DateTimeOffset? timestamp = null)
    {
        var user = _userStore.FindByUsername(username);
        if (user is null)
        {
            return false;
        }

        return TotpAuthenticator.VerifyCode(user.TotpSecret, code, timestamp: timestamp);
    }

    public bool Authenticate(string username, string password, string code, DateTimeOffset? timestamp = null)
    {
        return ValidatePassword(username, password)
            && ValidateTotp(username, code, timestamp);
    }

    public string ResetTotpSecret(string username)
    {
        var user = _userStore.FindByUsername(username);
        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var newSecret = TotpAuthenticator.GenerateSecret();
        _userStore.UpdateTotpSecret(username, newSecret);
        return newSecret;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32);
    }
}

public sealed record RegistrationResult(bool IsNewUser, string? Secret);
public sealed record DemoUser(string Username, byte[] PasswordSalt, byte[] PasswordHash, string TotpSecret);
