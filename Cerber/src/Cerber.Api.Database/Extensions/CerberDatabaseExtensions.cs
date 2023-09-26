using Cerber.Api.Database.EntityFramework;
using Cerber.Api.Database.Infrastructure;
using Cerber.Api.Database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Cerber.Api.Database.Extensions;

public static class CerberDatabaseExtensions
{
    internal static void ConfigureDatabaseOptionsBuilder(this DbContextOptionsBuilder builder, DatabaseOptions options)
    {
        switch (options.DatabaseType)
        {
            case DatabaseType.MySql: 
                builder.UseMySql(new MySqlConnection(options.ConnectionString), ServerVersion.AutoDetect(options.ConnectionString));
                break;
            case DatabaseType.SqlServer:
                builder.UseSqlServer(options.ConnectionString);
                break;
            default:
                throw new InvalidOperationException($"DatabaseType {options.DatabaseType} is not valid");
        }
    }
    
    private static void AddCerberDatabase(this IServiceCollection services, Action<DatabaseOptions>? configure)
    {
        var options = new DatabaseOptions();
        configure?.Invoke(options);
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException($"ConnectionStrings:Cerber is null or empty");
        }
        services.AddDbContext<CerberDbContext>(builder =>
        {
            builder.ConfigureDatabaseOptionsBuilder(options);
        });
    }
    
    public static void AddCerberDataContext(this IServiceCollection services, Action<DatabaseOptions>? configure)
    {
        services.AddCerberDatabase(configure);
        services.AddScoped<IHeartbeatRepository, HeartbeatRepository>();
    }
}