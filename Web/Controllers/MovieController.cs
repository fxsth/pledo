using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.Services;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovieController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<MovieController> _logger;

    public MovieController(UnitOfWork unitOfWork, ILogger<MovieController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<Movie>> Get([FromQuery] string libraryId)
    {
        return await _unitOfWork.MovieRepository.Get(x => x.LibraryId == libraryId, includeProperties:nameof(Movie.MediaFiles));
    }
}