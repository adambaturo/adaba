using Adaba.Infrastructure.Database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace Adaba.Infrastructure.Database.Extensions;

public static class DatabaseContextExtensions
{
    public static DbContextOptionsBuilder ConfigureDatabaseContext(this DbContextOptionsBuilder builder, IDatabaseOptions options)
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
        builder.EnableSensitiveDataLogging(options.SensitiveDataLogging);
        return builder;
    }

    public static IServiceCollection AddDatabaseContext<TContext>(this IServiceCollection services, Action<DatabaseOptions>? configure)
        where TContext : DbContext
    {
        var options = new DatabaseOptions();
        configure?.Invoke(options);
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            throw new InvalidOperationException($"ConnectionString for {typeof(TContext)} is null or empty");
        }
        services.AddDbContext<TContext>(builder => { builder.ConfigureDatabaseContext(options); });
        return services;
    }

    public static IServiceCollection AddDatabaseRepository<TRepo, TRepoImplementation>(this IServiceCollection services)
        where TRepo : class
        where TRepoImplementation : class, TRepo
    {
        services.AddScoped<TRepo, TRepoImplementation>();
        return services;
    }
}