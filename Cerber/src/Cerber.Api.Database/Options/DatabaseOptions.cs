namespace Cerber.Api.Database.Options;

public class DatabaseOptions
{
    public const string Name = "Cerber";
    public string? ConnectionString { get; set; }
    public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;
}