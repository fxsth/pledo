using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<AccountController> _logger;

    public LibraryController(UnitOfWork unitOfWork, ILogger<AccountController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public Task<IEnumerable<Library>> Get([FromQuery] string server, [FromQuery] string? mediaType = null)
    {
        if (mediaType == null)
            return Task.FromResult(_unitOfWork.LibraryRepository.Get(library => library.ServerId == server));
        else
            return Task.FromResult(_unitOfWork.LibraryRepository.Get(library => library.ServerId == server && library.Type == mediaType));
    }
}