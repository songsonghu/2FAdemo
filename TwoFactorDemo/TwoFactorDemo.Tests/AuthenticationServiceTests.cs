using TwoFactorDemo;

namespace TwoFactorDemo.Tests;

public class AuthenticationServiceTests
{
    private const string Secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    [Fact]
    public void GenerateCode_UsesKnownTotpVector()
    {
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(59);

        var code = TotpAuthenticator.GenerateCode(Secret, timestamp);

        Assert.Equal("287082", code);
    }

    [Fact]
    public void Authenticate_ReturnsTrue_WhenPasswordAndTotpAreValid()
    {
        var sut = new AuthenticationService();
        const string username = "demo";
        const string password = "P@ssw0rd123!";
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(59);
        sut.AddUser(username, password, Secret);
        var code = TotpAuthenticator.GenerateCode(Secret, timestamp);

        var result = sut.Authenticate(username, password, code, timestamp);

        Assert.True(result);
    }

    [Fact]
    public void Authenticate_ReturnsFalse_WhenTotpIsInvalid()
    {
        var sut = new AuthenticationService();
        sut.AddUser("demo", "P@ssw0rd123!", Secret);

        var result = sut.Authenticate("demo", "P@ssw0rd123!", "000000", DateTimeOffset.FromUnixTimeSeconds(59));

        Assert.False(result);
    }
}
