using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<AccountsController> _logger;

    public DownloadController(IDownloadService downloadService, ILogger<AccountsController> logger)
    {
        _downloadService = downloadService;
        _logger = logger;
    }

    [HttpGet]
    public async Task Download([FromBody] string key)
    {
        await _downloadService.Download(key);
    }
}