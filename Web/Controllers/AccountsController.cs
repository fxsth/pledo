using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(ISettingsService settingsService, ILogger<AccountsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Account>> Get()
    {
        return await _settingsService.GetPlexAccounts();
    }
    
    [HttpPost]
    public async Task Add([FromBody] Credentials credentials)
    {
        await _settingsService.AddPlexAccount(credentials);
    }
    
    [HttpDelete("{username}")]
    public async Task Delete(string username)
    {
        await _settingsService.RemovePlexAccount(username);
    }
}