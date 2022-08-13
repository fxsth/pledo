﻿using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServerController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<AccountsController> _logger;

    public ServerController(ISettingsService settingsService, ILogger<AccountsController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Server>> Get()
    {
        return await _settingsService.GetServers();
    }
}