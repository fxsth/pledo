using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ISettingsService settingsService, ILogger<AccountController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    [Route("loginuri")]
    public async Task<string> LoginUri()
    {
        UriBuilder uriBuilder = new UriBuilder()
        {
            Host = HttpContext.Request.Host.Host,
            Port = HttpContext.Request.Host.Port.Value,
            Scheme = HttpContext.Request.Scheme
        };
        return await _settingsService.GeneratePlexAuthUrl(uriBuilder.Uri);
    }
    
    [HttpGet]
    public async Task<Account?> Get()
    {
        return await _settingsService.GetPlexAccount();
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