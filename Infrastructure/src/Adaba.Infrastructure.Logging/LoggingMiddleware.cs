using System.Buffers;
using System.IO.Pipelines;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace Adaba.Infrastructure.Logging;

public class LoggingMiddleware
{
    private const string Unreadable = "<unreadable>";
    private const string Empty = "<empty>";
    private const string Disabled = "<disabled>";
    private const string Unsupported = "<unsupported>";
    private const string RequestBody = "RequestBody";
    private const string ResponseBody = "ResponseBody";
    private const string ResponseException = "ResponseException";
    private const string More = " [more...]";

    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<LoggingOptions> _options;

    public LoggingMiddleware(RequestDelegate next, IOptionsMonitor<LoggingOptions> loggingOptions)
    {
        _next = next;
        _options = loggingOptions;
    }

    private static async Task<string> ReadFromPipe(PipeReader reader, int maxLength)
    {
        long bytesCount = 0;
        var builder = new StringBuilder();
        while (true)
        {
            var readResult = await reader.ReadAsync();
            if (readResult.Buffer.Length > 0)
            {
                bytesCount += readResult.Buffer.Length;
                builder.Append(Encoding.UTF8.GetString(readResult.Buffer.ToArray(), 0, maxLength));
            }
            reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
            if (readResult.IsCanceled || readResult.IsCompleted || bytesCount > maxLength + 1)
            {
                break;
            }
        }
        if (bytesCount <= 0)
        {
            return Unreadable;
        }
        if (bytesCount > maxLength)
        {
            builder.Append(More);
        }
        return builder.ToString();
    }

    private static async Task<string> ReadFromStream(Stream stream, int maxLength)
    {
        if (!stream.CanSeek || !stream.CanRead)
        {
            return Unsupported;
        }
        if (stream.Length <= 0)
        {
            return Empty;
        }
        try
        {
            var buffer = new byte[maxLength + 1].AsMemory();
            stream.Seek(0, SeekOrigin.Begin);
            var bytesCount = await stream.ReadAsync(buffer);
            if (bytesCount <= 0)
            {
                return Unreadable;
            }
            var builder =
                new StringBuilder(Encoding.UTF8.GetString(buffer.ToArray(), 0, int.Min(bytesCount, maxLength)));
            if (bytesCount > maxLength)
            {
                builder.Append(More);
            }
            return builder.ToString();
        }
        finally
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    private static string GetExceptionInfo(Exception exc)
    {
        return $"<exception \"{exc.GetType()}\", message \"{exc.Message}\"";
    }

    private async Task<string> GetRequestBody(HttpContext httpContext)
    {
        var request = httpContext.Request;
        var maxRequestBodyLog = int.Min(_options.CurrentValue.MaxRequestBodyLog,
            int.Max(unchecked((int)(request.ContentLength ?? 0)), 0));
        if (_options.CurrentValue.MaxResponseBodyLog <= 0)
        {
            return Disabled;
        }
        if ((request.ContentLength ?? 0) <= 0)
        {
            return Empty;
        }
        try
        {
            return FormatString(await ReadFromPipe(request.BodyReader, maxRequestBodyLog),
                _options.CurrentValue.RemoveLineBreaksFromRequest);
        }
        catch (Exception exc)
        {
            return GetExceptionInfo(exc);
        }
    }

    private async Task<string> GetResponseBody(HttpContext? httpContext)
    {
        var maxResponseBodyLog = _options.CurrentValue.MaxResponseBodyLog;
        if (httpContext == null || maxResponseBodyLog <= 0)
        {
            return Disabled;
        }
        var response = httpContext.Response;
        try
        {
            return FormatString(await ReadFromStream(response.Body, maxResponseBodyLog),
                _options.CurrentValue.RemoveLineBreaksFromResponse);
        }
        catch (Exception exc)
        {
            return GetExceptionInfo(exc);
        }
    }

    private static string FormatString(string input, bool removeLineBreaks)
    {
        return removeLineBreaks ? input.ReplaceLineEndings(string.Empty) : input;
    }

    public async Task Invoke(HttpContext httpContext, IDiagnosticContext diagnosticContext)
    {
        diagnosticContext.Set(RequestBody, await GetRequestBody(httpContext));
        try
        {
            if (_options.CurrentValue.MaxResponseBodyLog <= 0)
            {
                diagnosticContext.Set(ResponseBody, await GetResponseBody(null));
                await _next(httpContext);
            }
            else
            {
                var originalBody = httpContext.Response.Body;
                using var interceptedBody = new MemoryStream();
                httpContext.Response.Body = interceptedBody;
                try
                {
                    await _next(httpContext);
                    diagnosticContext.Set(ResponseBody, await GetResponseBody(httpContext));
                }
                finally
                {
                    await interceptedBody.CopyToAsync(originalBody);
                }
            }
        }
        catch (Exception exc)
        {
            diagnosticContext.Set(ResponseException, GetExceptionInfo(exc));
            throw;
        }
    }
}