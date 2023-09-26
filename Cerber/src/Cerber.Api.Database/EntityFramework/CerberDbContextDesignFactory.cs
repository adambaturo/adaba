using Cerber.Api.Database.Extensions;
using Cerber.Api.Database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cerber.Api.Database.EntityFramework;

public class CerberDbContextDesignFactory : IDesignTimeDbContextFactory<CerberDbContext>
{
    public CerberDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        var options = new DatabaseOptions
        {
            DatabaseType = DatabaseType.SqlServer,
            ConnectionString = configuration.GetConnectionString(DatabaseOptions.Name)
        };
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        { 
            throw new InvalidOperationException($"Cannot find ENV variable ConnectionStrings__{DatabaseOptions.Name}");
        }
        var optionsBuilder = new DbContextOptionsBuilder<CerberDbContext>();
        optionsBuilder.ConfigureDatabaseOptionsBuilder(options);
        return new CerberDbContext(optionsBuilder.Options);
    }
}
