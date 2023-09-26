using Cerber.Api.Database.Model;
using Cerber.Api.Host.Dto;

namespace Cerber.Api.Host.Extensions;

public static class MapExtensions
{
    public static HeartbeatInfo MapToHeartbeatInfo(this HeartbeatEntity entity)
    {
        var toMap = entity ?? throw new ArgumentNullException(nameof(entity));
        return new HeartbeatInfo
        {
            Product = toMap.Product.Name,
            Instance = toMap.Instance,
            Version = toMap.Version,
            ClientTimestamp = toMap.ClientTimestamp,
            ServerTimestamp = toMap.ServerTimestamp,
            Id = toMap.Id
        };
    }
}