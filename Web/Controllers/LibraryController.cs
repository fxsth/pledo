using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AccountsController> _logger;

    public LibraryController(ISettingsService settingsService, ILogger<AccountsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Library>> Get()
    {
        return await _settingsService.GetLibraries();
    }
}