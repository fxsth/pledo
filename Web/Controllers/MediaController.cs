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

    [HttpGet("movie")]
    public async Task<IEnumerable<Movie>> GetMovies([FromQuery] MediaQueryParameter queryParameter)
    {
        return await _unitOfWork.MovieRepository.Get(x => x.LibraryId == queryParameter.LibraryId,
            includeProperties: nameof(Movie.MediaFiles),
            offset: (queryParameter.PageNumber - 1) * queryParameter.PageSize,
            size: queryParameter.PageSize);
    }

    [HttpGet("tvshow")]
    public async Task<IEnumerable<TvShow>> GetTvShows([FromQuery] MediaQueryParameter queryParameter)
    {
        return await _unitOfWork.TvShowRepository.Get(x => x.LibraryId == queryParameter.LibraryId,
            s => s.OrderBy(x => x.Title), 
            nameof(TvShow.Episodes),
            offset: (queryParameter.PageNumber - 1) * queryParameter.PageSize,
            size: queryParameter.PageSize);
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchResultResource>> Search([FromQuery] string searchTerm)
    {
        SearchResultResource result = new();

        var search = searchTerm.Trim('%').Insert(0, "%");
        search = search.Insert(search.Length, "%");
        var movies = await _unitOfWork.MovieRepository.Get(x => EF.Functions.Like(x.Title, search),
            includeProperties: nameof(Movie.MediaFiles));
        IEnumerable<Movie> movieList = movies.ToList();
        result.Movies = movieList.Take(100);
        result.TotalMoviesMatching = movieList.Count();

        return result;
    }
}