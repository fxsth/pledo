using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TvShowController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;

    public TvShowController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IEnumerable<TvShow>> Get([FromQuery] string libraryId)
    {
        return await _unitOfWork.TvShowRepository.Get(x=>x.LibraryId == libraryId, s=>s.OrderBy(x=>x.Title), nameof(TvShow.Episodes));
    }
    
    [HttpGet("tvShowId")]
    public async Task<TvShow?> GetById(string tvShowId)
    {
        return await _unitOfWork.TvShowRepository.GetById(tvShowId);
    }
}