using Microsoft.EntityFrameworkCore;
using SurveySystem.Catalog.Infrastructure.Data;
using System.Diagnostics;

namespace SurveySystem.Catalog.MigrationService;

public class ApiDbInitializer(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            //AddHostedService registers ApiDbInitializer as a singleton
            // So to consume a scoped service from this singleton service 
            // We need to create a scope first with IServiceProvider, then
            // Get the required service from the scope
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            await RunMigrationAsync(dbContext, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task RunMigrationAsync(CatalogContext dbContext, CancellationToken cancellationToken)
    {
        // using strategy to handle failures like network issues
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }
}