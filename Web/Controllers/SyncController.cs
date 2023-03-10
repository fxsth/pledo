using Microsoft.AspNetCore.Mvc;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly ILogger<AccountController> _logger;

    public SyncController(ISyncService syncService, ILogger<AccountController> logger)
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
            await _syncService.SyncAll();
    }
}