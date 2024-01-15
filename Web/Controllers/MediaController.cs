using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using Web.Models.DTO;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<MediaController> _logger;

    public MediaController(UnitOfWork unitOfWork, ILogger<MediaController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResultResource>> Get([FromQuery] MediaQueryParameter queryParameter)
    {
        SearchResultResource result = new();
        if (queryParameter.SearchTerm == null)
            return BadRequest();
        if (queryParameter.SearchTerm != null)
        {
            var searchTerm = queryParameter.SearchTerm.Trim('%').Insert(0, "%");
            searchTerm = searchTerm.Insert(searchTerm.Length,"%");
            var movies =  await _unitOfWork.MovieRepository.Get(x => EF.Functions.Like(x.Title, searchTerm),
                includeProperties: nameof(Movie.MediaFiles));
            IEnumerable<Movie> movieList = movies.ToList();
            result.Movies = movieList.Take(100);
            result.TotalMoviesMatching = movieList.Count();
        }
        return result;
    }
    [HttpGet("ping")]
    public async Task<ActionResult> Get()
    {

        return Ok();
    }
}