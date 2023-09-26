using Cerber.Api.Database.EntityFramework;
using Cerber.Api.Database.Model;

namespace Cerber.Api.Database.Infrastructure;

public class HeartbeatRepository : IHeartbeatRepository
{
    private readonly CerberDbContext _context;

    public HeartbeatRepository(CerberDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async ValueTask<HeartbeatEntity?> GetAsync(long id)
    {
        return await _context.Heartbeats.FindAsync(id);
    }
}