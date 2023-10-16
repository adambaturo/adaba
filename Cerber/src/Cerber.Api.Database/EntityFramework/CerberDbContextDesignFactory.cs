using Adaba.Infrastructure.Database.Extensions;
using Adaba.Infrastructure.Database.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Cerber.Api.Database.EntityFramework;

public class CerberDbContextDesignFactory : IDesignTimeDbContextFactory<CerberDbContext>
{
    private const string DesignConnectionStringKey = "Cerber";
    public CerberDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
        var options = new DatabaseOptions
        {
            DatabaseType = DatabaseType.SqlServer,
            ConnectionString = configuration.GetConnectionString(DesignConnectionStringKey)
        };
        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        { 
            throw new InvalidOperationException($"Cannot find ENV variable ConnectionStrings__{DesignConnectionStringKey}");
        }
        var optionsBuilder = new DbContextOptionsBuilder<CerberDbContext>();
        optionsBuilder.ConfigureDatabaseContext(options);
        return new CerberDbContext(optionsBuilder.Options);
    }
}
