using System.Reflection;
using Serilog;

namespace Adaba.Infrastructure.Logging;

public class StartupLoggingScope : IDisposable
{
    public static StartupLoggingScope Create()
    {
        return  new StartupLoggingScope().InitializeStandardBootstrapper();
    }

    public int InitializeAndRun(Func<StartupLoggingScope, int> initializeAndRunFunc)
    {
        try
        {
            return initializeAndRunFunc(this);
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Terminated {Program}", _program);
            return 0xFF;
        }
    }

    private readonly string _program;

    private StartupLoggingScope()
    {
        var rootAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        _program = rootAssembly.GetName().Name ?? Environment.ProcessPath ?? nameof(_program);
    }
    
    private StartupLoggingScope InitializeStandardBootstrapper()
    {
        Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();
        Log.Information("Starting {Program}", _program);
        return this;
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }
        Log.Information("Finished {Program}", _program);
        Log.CloseAndFlush();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public static string? EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
}