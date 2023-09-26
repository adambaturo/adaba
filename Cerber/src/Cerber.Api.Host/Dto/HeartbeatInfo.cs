namespace Cerber.Api.Host.Dto;

/// <summary>
/// Represents heartbeat info
/// </summary>
public class HeartbeatInfo
{
    public long Id { get; set; }
    /// <summary>
    /// Product name
    /// </summary>
    public string Product { get; set; } = null!;
    /// <summary>
    /// Product version
    /// </summary>
    public string? Version { get; set; }
    /// <summary>
    /// Instance
    /// </summary>
    public string? Instance { get; set; }
    /// <summary>
    /// Client timestamp
    /// </summary>
    public DateTimeOffset? ClientTimestamp { get; set; }
    /// <summary>
    /// Server timestamp
    /// </summary>
    public DateTimeOffset ServerTimestamp { get; set; } 
}