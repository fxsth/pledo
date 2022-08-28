using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<TaskController> _logger;
    private readonly ISyncService _syncService;

    public TaskController(IDownloadService downloadService, ILogger<TaskController> logger, ISyncService syncService)
    {
        _downloadService = downloadService;
        _logger = logger;
        _syncService = syncService;
    }

    [HttpGet]
    public IEnumerable<BusyTask> Get()
    {
        List<BusyTask> busyTasks = new List<BusyTask>();
        var currentSyncTask = _syncService.GetCurrentSyncTask();
        if (currentSyncTask != null)
            busyTasks.Add(currentSyncTask);
        busyTasks.AddRange(_downloadService.PendingDownloads.Select(x => new BusyTask()
        {
            Id = x.Id,
            Name = x.Name,
            Type = TaskType.Downloading,
            Progress = (double)x.DownloadedBytes / x.TotalBytes,
            Completed = x.FinishedSuccessfully
        }));
        return busyTasks;
    }
}