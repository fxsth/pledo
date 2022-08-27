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

    public TaskController(IDownloadService downloadService, ILogger<TaskController> logger)
    {
        _downloadService = downloadService;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<BusyTask> Get()
    {
        return _downloadService.PendingDownloads.Select(x=>new BusyTask()
        {
            Id = x.Id,
            Name = x.Name,
            Type = TaskType.Downloading,
            Progress = (double) x.DownloadedBytes/x.TotalBytes,
            Completed = x.FinishedSuccessfully
        });
    }
}