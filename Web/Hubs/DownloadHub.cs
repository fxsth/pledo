using Microsoft.AspNetCore.SignalR;
using Web.Services;

namespace Web.Hubs;

public class DownloadHub : Hub
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<DownloadHub> _logger;

    public DownloadHub(IDownloadService downloadService, ILogger<DownloadHub> logger)
    {
        _downloadService = downloadService;
        _logger = logger;
        _downloadService.PendingDownloadCountChanged += async x=> await UpdatePendingDownload(x);
    }

    private async Task UpdatePendingDownload(int numberOfPendingDownloads)
    {
        await Clients.All.SendAsync("updatePendingDownloads", numberOfPendingDownloads);
    }
}