using Adaba.Infrastructure.Logging;
using Asp.Versioning;
using Cerber.Api.Database.Extensions;
using Cerber.Api.Database.Options;
using Microsoft.OpenApi.Models;

using var loggingScope = StartupLoggingScope.Create();
return loggingScope.InitializeAndRun(scope =>
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddStandardLogging(StartupLoggingScope.EnvironmentName);
    builder.AddStandardHttpLogging();
    builder.Configuration.AddCommandLine(args).AddEnvironmentVariables();
    builder.Services.AddControllers();
    var versioningBuilder = builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader =
            ApiVersionReader.Combine(new QueryStringApiVersionReader(), new HeaderApiVersionReader());
    });
    versioningBuilder.AddApiExplorer(options => { options.GroupNameFormat = "\'v\'VVV"; });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Version = "1.0", Title = "Cerber API ver. 1.0" });
        options.SchemaGeneratorOptions.SupportNonNullableReferenceTypes = true;
        options.ResolveConflictingActions(apiDesc => apiDesc.First());
    });
    builder.Services.AddCerberDataContext(options =>
    {
        options.DatabaseType = DatabaseType.SqlServer;
        options.ConnectionString = builder.Configuration.GetConnectionString(DatabaseOptions.Name);
    });
    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cerber API ver. 1.0");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Cerber API ver. 2.0");
    });
    app.UseStandardHttpLogging();
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
    return 0x00;
});