using Microsoft.AspNetCore.Mvc;
using Web.Models;
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
    public async Task Sync(SyncType syncType)
    {
        if (_syncService.GetCurrentSyncTask() != null)
        {
            Conflict("Sync is already ongoing");
        }

        switch (syncType)
        {
            case SyncType.Full:
                await _syncService.SyncAll();
                break;
            case SyncType.Connection:
                await _syncService.SyncConnections();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(syncType), syncType, null);
        }

        
    }
}