using Asp.Versioning;
using Cerber.Api.Host.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Cerber.Api.Host.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class HeartbeatsController : ControllerBase
{
    public HeartbeatsController()
    {
    }

    [HttpGet("")]
    public IAsyncEnumerable<HeartbeatInfo> Fetch()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:long:required}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<HeartbeatInfo> GetById([FromRoute] long id)
    {
        return new HeartbeatInfo
        {
            Id = 1,
            Product = "test"
        };
    }
    
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<HeartbeatInfo> Register([FromBody] Heartbeat heartbeatRequest)
    {
        return new HeartbeatInfo
        {
            Id = 1,
            Product = "test"
        };
    }
    
    [HttpPost("{product:required}/{instance}/{version}")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ValueTask<ActionResult<HeartbeatInfo>> Register([FromRoute] string product, [FromRoute] string? instance = null, [FromRoute] string? version = null, [FromBody] DateTimeOffset? clientTimestamp = null)
    {
        throw new NotImplementedException();
    }
}