namespace Adaba.Infrastructure.Logging.Tests;

public class LoggingOptionsTests
{
    [Test]
    public void WhenOptionsCreated_DefaultValuesShouldBeSet()
    {
        var sut = new LoggingOptions();
        Assert.Multiple(() =>
        {
            Assert.That(sut.RemoveLineBreaksFromRequest, Is.True);
            Assert.That(sut.RemoveLineBreaksFromResponse, Is.True);
            Assert.That(sut.IncludeQueryInRequestPath, Is.True);
            Assert.That(sut.MaxRequestBodyLog, Is.EqualTo(256));
            Assert.That(sut.MaxResponseBodyLog, Is.EqualTo(256));
            Assert.That(string.IsNullOrWhiteSpace(sut.MessageTemplate), Is.False);
        });
    }
}