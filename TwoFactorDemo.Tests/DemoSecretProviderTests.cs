using TwoFactorDemo;

namespace TwoFactorDemo.Tests;

public class UserRegistrationTests
{
    [Fact]
    public void RegisterUser_CreatesNewUser_WithSecret()
    {
        var sut = new AuthenticationService();

        var result = sut.RegisterUser("demo", "P@ssw0rd123!");

        Assert.True(result.IsNewUser);
        Assert.False(string.IsNullOrWhiteSpace(result.Secret));
    }

    [Fact]
    public void RegisterUser_DoesNotRotateSecret_ForExistingUser()
    {
        var sut = new AuthenticationService();
        var first = sut.RegisterUser("demo", "P@ssw0rd123!");

        var second = sut.RegisterUser("demo", "P@ssw0rd123!");

        Assert.True(first.IsNewUser);
        Assert.False(second.IsNewUser);
        Assert.Null(second.Secret);
        Assert.NotNull(first.Secret);
        var code = TotpAuthenticator.GenerateCode(first.Secret!, DateTimeOffset.FromUnixTimeSeconds(59));
        Assert.True(sut.ValidateTotp("demo", code, DateTimeOffset.FromUnixTimeSeconds(59)));
    }

    [Fact]
    public void ResetTotpSecret_ChangesSecret()
    {
        var sut = new AuthenticationService();
        var register = sut.RegisterUser("demo", "P@ssw0rd123!");

        var newSecret = sut.ResetTotpSecret("demo");

        Assert.NotEqual(register.Secret, newSecret);
    }
}
