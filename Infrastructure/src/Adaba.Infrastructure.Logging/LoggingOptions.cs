namespace Adaba.Infrastructure.Logging;

public class LoggingOptions
{
    public string MessageTemplate { get; set; } =
        "Req: \"{RequestMethod} {RequestPath}\", Headers: \"{RequestHeaders}\", Body: \"{RequestBody}\" -> Resp: {StatusCode}, Headers: \"{ResponseHeaders}\", Body: \"{ResponseBody}\", Elapsed: {Elapsed:0.0000}ms";
    public bool IncludeQueryInRequestPath { get; set; } = true;
    public int MaxRequestBodyLog { get; set; } = 256;
    public bool RemoveLineBreaksFromRequest { get; set; } = true;
    public int MaxResponseBodyLog { get; set; } = 256;
    public bool RemoveLineBreaksFromResponse { get; set; } = true;
}