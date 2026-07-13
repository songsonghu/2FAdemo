using TwoFactorDemo;

namespace TwoFactorDemo.Tests;

public class DemoSecretProviderTests
{
    [Fact]
    public void ResolveSecret_ReturnsFixedSecret_InDevelopment()
    {
        var secret = DemoSecretProvider.ResolveSecret("Development");

        Assert.Equal(DemoSecretProvider.DevelopmentFixedSecret, secret);
    }

    [Fact]
    public void ResolveSecret_ReturnsFixedSecret_InDevelopmentCaseInsensitive()
    {
        var secret = DemoSecretProvider.ResolveSecret("development");

        Assert.Equal(DemoSecretProvider.DevelopmentFixedSecret, secret);
    }

    [Fact]
    public void ResolveSecret_ReturnsGeneratedSecret_OutsideDevelopment()
    {
        var secret = DemoSecretProvider.ResolveSecret("Production");

        Assert.NotEqual(DemoSecretProvider.DevelopmentFixedSecret, secret);
        Assert.False(string.IsNullOrWhiteSpace(secret));
    }
}
