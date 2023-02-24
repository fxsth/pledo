using Microsoft.AspNetCore.Mvc;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<SettingController> _logger;

    public SettingController(ISettingsService settingsService,ILogger<SettingController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task UpdateSettings([FromBody] IEnumerable<SettingsResource> settings)
    {
        await _settingsService.ValidateSettings(settings);
        await _settingsService.UpdateSettings(settings);
    }
    
    [HttpGet]
    public async Task<IEnumerable<SettingsResource>> GetSettings()
    {
        return await _settingsService.GetSettings();
    }

    [HttpDelete]
    public async Task ResetDatabase()
    {
        await _settingsService.ResetDatabase();
    }
}