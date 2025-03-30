using Microsoft.EntityFrameworkCore;
using TokenCardCare.Server.Entity;

namespace TokenCardCare.Server.Service;

public class MigrationService(IServiceProvider serviceProvider, ILogger<MigrationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync(stoppingToken).ConfigureAwait(false);
        if (dbContext.Database.HasPendingModelChanges())
        {
            logger.LogInformation("start database migration...");
            await dbContext.Database.MigrateAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
            logger.LogInformation("database migration completed.");
        }
    }
}
