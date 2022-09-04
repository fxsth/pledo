using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<AccountController> _logger;

    public DownloadController(IDownloadService downloadService, ILogger<AccountController> logger)
    {
        _downloadService = downloadService;
        _logger = logger;
    }

    [HttpPost("movie/{key}")]
    public async Task DownloadMovie( string key)
    {
        await _downloadService.DownloadMovie(key);
    }
    
    [HttpPost("episode/{key}")]
    public async Task DownloadEpisode( string key)
    {
        await _downloadService.DownloadEpisode(key);
    }
}