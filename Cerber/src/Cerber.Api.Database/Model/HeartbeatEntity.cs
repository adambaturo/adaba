namespace Cerber.Api.Database.Model;

public class HeartbeatEntity
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public string? Version { get; set; }
    public string? Instance { get; set; }
    public DateTimeOffset? ClientTimestamp { get; set; }
    public DateTimeOffset ServerTimestamp { get; set; }
}