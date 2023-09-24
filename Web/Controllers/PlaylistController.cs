using Microsoft.AspNetCore.Mvc;
using Web.Data;
using Web.Models;
using Web.Models.DTO;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;

    public PlaylistController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IEnumerable<PlaylistResource>> Get()
    {
        var playlists =  _unitOfWork.PlaylistRepository.Get(includeProperties: nameof(Playlist.Server));
        List<PlaylistResource> playlistResources = new List<PlaylistResource>();
        foreach (var playlist in playlists)
        {
            var movies = _unitOfWork.MovieRepository.Get(x => playlist.Items.Contains(x.RatingKey)).ToDictionary(x=>x.RatingKey);
            var episodes = _unitOfWork.EpisodeRepository.Get(x => playlist.Items.Contains(x.RatingKey), includeProperties: nameof(Episode.TvShow)).ToDictionary(x=>x.RatingKey);

            List<PlaylistItem> items = playlist.Items.Select(x =>
            {
                if (movies.TryGetValue(x, out Movie movie))
                    return new PlaylistItem() { Id = x, Name = $"{ movie.Title } ({movie.Year})", Type = ElementType.Movie };
                else if (episodes.TryGetValue(x, out Episode episode))
                    return new PlaylistItem() { Id = x, Name = $"{ episode.Title } ({episode.TvShow.Title} S{episode.SeasonNumber}E{episode.EpisodeNumber})", Type = ElementType.Movie };
                else
                    return new PlaylistItem() { Id = x };
            }).ToList();
            playlistResources.Add(new PlaylistResource()
            {
                Items = items,
                Id = playlist.Id,
                Name = playlist.Name, 
                Server = playlist.Server,
                ServerId = playlist.ServerId
            });
        }
        
        return playlistResources;
    }
}