using Web.Models;

namespace Web.Services;

public class PeriodicallySyncBackgroundService :  IHostedService, IDisposable
{
    private readonly ILogger<PeriodicallySyncBackgroundService> _logger;
    private readonly ISyncService _syncService;
    private Timer? _timer = null;

    public PeriodicallySyncBackgroundService(ILogger<PeriodicallySyncBackgroundService> logger,
        ISyncService syncService)
    {
        _logger = logger;
        _syncService = syncService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogTrace("Background sync service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromHours(6));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        _syncService.Sync(SyncType.Connection);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background sync service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}