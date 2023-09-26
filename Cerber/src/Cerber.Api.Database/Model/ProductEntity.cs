namespace Cerber.Api.Database.Model;

public class ProductEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<HeartbeatEntity> Heartbeats { get; } = null!;
}