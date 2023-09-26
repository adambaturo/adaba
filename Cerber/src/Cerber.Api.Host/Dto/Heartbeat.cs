namespace Cerber.Api.Host.Dto;

/// <summary>
/// Represents heartbeat request
/// </summary>
public class Heartbeat
{
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
}