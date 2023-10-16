namespace Adaba.Infrastructure.Logging.Tests;

public class LoggingOptionsTests
{
    [Fact]
    public void WhenOptionsCreated_DefaultValuesShouldBeSet()
    {
        var sut = new HttpLoggingOptions();
        Assert.Multiple(() =>
        {
            Assert.Multiple(
                () => Assert.True(sut.RemoveLineBreaksFromRequest),
                () => Assert.True(sut.RemoveLineBreaksFromResponse),
                () => Assert.True(sut.IncludeQueryInRequestPath),
                () => Assert.Equal(256, sut.MaxRequestBodyLog),
                () => Assert.Equal(256, sut.MaxResponseBodyLog),
                () => Assert.False(string.IsNullOrWhiteSpace(sut.MessageTemplate))
            );
        });
    }
}