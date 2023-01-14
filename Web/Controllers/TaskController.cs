using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ILogger<TaskController> _logger;
    private readonly ISyncService _syncService;

    public TaskController(ILogger<TaskController> logger, ISyncService syncService)
    {
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
        return busyTasks;
    }
}