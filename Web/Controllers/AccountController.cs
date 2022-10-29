using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILoginService loginService, ILogger<AccountController> logger)
    {
        _loginService = loginService;
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
        return await _loginService.GeneratePlexAuthUrl(uriBuilder.Uri);
    }
    
    [HttpGet]
    public async Task<Account?> Get()
    {
        return await _loginService.GetPlexAccount();
    }
    
    [HttpPost]
    public async Task Add([FromBody] CredentialsResource credentialsResource)
    {
        await _loginService.AddPlexAccount(credentialsResource);
    }
    
    [HttpDelete("{username}")]
    public async Task Delete(string username)
    {
        await _loginService.RemovePlexAccount(username);
    }
}