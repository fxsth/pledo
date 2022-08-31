﻿using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<TvShowController> _logger;

    public MovieController(ISettingsService settingsService, ILogger<TvShowController> logger)
    {
        _settingsService = settingsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Movie>> Get([FromQuery] string libraryId)
    {
        return await _settingsService.GetMovies(libraryId);
    }
}