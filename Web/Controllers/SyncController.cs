using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly ILogger<AccountsController> _logger;

    public SyncController(ISyncService syncService, ILogger<AccountsController> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    [HttpPost]
    public async Task Sync()
    {
        if (_syncService.GetCurrentSyncTask() != null)
            Conflict("Sync is already ongoing");
        else
            await Task.Run(() => _syncService.SyncAll());
    }
}