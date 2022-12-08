using Microsoft.AspNetCore.Mvc;
using Web.Models.DTO;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DirectoryController : ControllerBase
{
    private readonly ILogger<DirectoryController> _logger;

    public DirectoryController(ILogger<DirectoryController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public Task<DirectoryResource> GetDirectories([FromQuery] string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (OperatingSystem.IsWindows())
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else if (OperatingSystem.IsLinux())
                path = "/";
        }

        IEnumerable<string> list;
        try
        {
            list = Directory.GetDirectories(path);
        }
        catch
        {
            list = Enumerable.Empty<string>();
        }
        return Task.FromResult(new DirectoryResource()
        {
            CurrentDirectory = path,
            SubDirectories = list
        });
    }
}