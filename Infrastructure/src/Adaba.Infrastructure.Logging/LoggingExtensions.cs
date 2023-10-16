using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace Adaba.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddStandardLogging(this WebApplicationBuilder webAppBuilder, string? environment)
    {
        webAppBuilder.Host.UseSerilog((_, _, config) =>
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ??
                             Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json");
            if (!string.IsNullOrWhiteSpace(environment))
            {
                builder.AddJsonFile($"appsettings.{environment}.json", true, true);
            }
            config.ReadFrom
                .Configuration(builder.Build())
                .Enrich.WithProperty("AppName", AppVersion.Value.AppName)
                .Enrich.WithProperty("AppVersion", AppVersion.Value.AppVersion)
                .Enrich.WithProperty("ProductVersion", AppVersion.Value.ProductVersion);
        });
        return webAppBuilder;
    }

    public static WebApplicationBuilder AddStandardHttpLogging(this WebApplicationBuilder webAppBuilder,
        string? configSectionName = default)
    {
        webAppBuilder.Services.Configure<HttpLoggingOptions>(
            webAppBuilder.Configuration.GetSection(configSectionName ?? "HttpLogging"));
        return webAppBuilder;
    }

    public static WebApplication UseStandardHttpLogging(this WebApplication app)
    {
        var httpLoggingOptions = app.Services.GetRequiredService<IOptions<HttpLoggingOptions>>();
        app.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = httpLoggingOptions.Value.IncludeQueryInRequestPath;
            options.EnrichDiagnosticContext = EnrichFromRequest;
            options.GetLevel = GetLogLevelForRequestLogging;
            options.MessageTemplate = httpLoggingOptions.Value.MessageTemplate;
        });
        app.UseMiddleware<LoggingMiddleware>();
        return app;
    }

    private static readonly Lazy<(string AppName, string AppVersion, string ProductVersion)> AppVersion = new(() =>
    {
        var appVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        return (
            appVersion.ProductName ?? string.Empty,
            appVersion.FileVersion ?? string.Empty,
            appVersion.ProductVersion ?? string.Empty);
    });

    private static LogEventLevel GetLogLevelForRequestLogging(HttpContext httpContext, double elapsedMilliseconds,
        Exception? exception)
    {
        if (exception != null || httpContext.Response.StatusCode > 499)
        {
            return LogEventLevel.Error;
        }
        return IsHealthCheckEndpoint(httpContext)
            ? LogEventLevel.Verbose
            : LogEventLevel.Information;
    }

    private static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;
        diagnosticContext.Set("Host", request.Host.ToString());
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }
        diagnosticContext.Set("ContentType", request.ContentType);
        var endpoint = httpContext.GetEndpoint();
        if (endpoint != null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
        diagnosticContext.Set("RequestHeaders",
            string.Join(";", httpContext.Request.Headers.Select(h => $"{h.Key}={h.Value}")));
        diagnosticContext.Set("ResponseHeaders",
            string.Join(";", httpContext.Response.Headers.Select(h => $"{h.Key}={h.Value}")));
    }

    private static bool IsHealthCheckEndpoint(HttpContext ctx)
    {
        var endpoint = ctx.GetEndpoint();
        return endpoint != null &&
               string.Equals(endpoint.DisplayName, "Health checks", StringComparison.OrdinalIgnoreCase);
    }
}