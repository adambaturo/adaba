using Cerber.Api.Database.Model;
using Cerber.Api.Host.Extensions;

namespace Cerber.Api.Host.Tests;

public class MapExtensionsTests
{
    [Test]
    public void WhenEntityIsNull_MapToHeartbeatInfoShallThrowException()
    {
        HeartbeatEntity entity = null!;
        Assert.Throws<ArgumentNullException>(() => entity.MapToHeartbeatInfo());
    }
}