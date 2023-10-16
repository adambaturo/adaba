namespace Adaba.Infrastructure.Database.Options;

public class DatabaseOptions : IDatabaseOptions
{
    public string? ConnectionString { get; set; }
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
    public bool SensitiveDataLogging { get; set; }
}

public interface IDatabaseOptions
{
    public string? ConnectionString { get; }
    public DatabaseType DatabaseType { get; }
    public bool SensitiveDataLogging { get; }
}