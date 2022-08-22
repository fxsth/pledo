using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<AccountsController> _logger;

    public TaskController(IDownloadService downloadService, ILogger<AccountsController> logger)
    {
        _downloadService = downloadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<BusyTask>> Get()
    {
        return _downloadService.PendingDownloads.Select(x=>new BusyTask()
        {
            Id = x.Id,
            Name = x.Name,
            Type = TaskType.Downloading
        });
    }
}