using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TvShowController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly ILogger<TvShowController> _logger;

    public TvShowController(UnitOfWork unitOfWork, ILogger<TvShowController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public Task<IEnumerable<TvShow>> Get([FromQuery] string libraryId)
    {
        return Task.FromResult(_unitOfWork.TvShowRepository.Get(x=>x.LibraryId == libraryId, s=>s.OrderBy(x=>x.Title), nameof(TvShow.Episodes)));
    }
    
    [HttpGet("tvShowId")]
    public async Task<TvShow?> GetById(string tvShowId)
    {
        return await _unitOfWork.TvShowRepository.GetById(tvShowId);
    }
}