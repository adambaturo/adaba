using Cerber.Api.Database.Model;

namespace Cerber.Api.Database.Infrastructure;

public interface IHeartbeatRepository
{
    public ValueTask<HeartbeatEntity?> GetAsync(long id);
}