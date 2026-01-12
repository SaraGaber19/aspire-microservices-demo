using SurveySystem.Catalog.Infrastructure;
using SurveySystem.Catalog.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructureServices();

builder.Services.AddHostedService<ApiDbInitializer>();

var host = builder.Build();
host.Run();