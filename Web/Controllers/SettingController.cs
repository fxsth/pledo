using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AccountController> _logger;

    public SettingController(ISettingsService settingsService,ILogger<AccountController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpPost]
    [Route("[controller]/directory")]
    public async Task SetDirectories([FromBody] Directories directories)
    {
        await _settingsService.UpdateDirectories(directories);
    }
    
    [HttpGet]
    [Route("[controller]/directory")]
    public async Task<Directories> GetDirectories()
    {
        return await _settingsService.GetDirectories();
    }
}