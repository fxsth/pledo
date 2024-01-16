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
    public async Task<IEnumerable<Library>> Get([FromQuery] string? server = null, [FromQuery] string? mediaType = null)
    {
        if (server == null)
            return await _unitOfWork.LibraryRepository.Get(mediaType == null? _=>true : library => library.Type == mediaType,
                includeProperties: nameof(Library.Server));

        return await _unitOfWork.LibraryRepository.Get(library => library.ServerId == server);
    }
}