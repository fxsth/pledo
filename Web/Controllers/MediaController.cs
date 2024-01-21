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
    public async Task<ResultResource<Movie>> GetMovies([FromQuery] MediaQueryParameter queryParameter)
    {
        var totalItems = await _unitOfWork.MovieRepository.Count(x => x.LibraryId == queryParameter.LibraryId);
        var items = await _unitOfWork.MovieRepository.Get(x => x.LibraryId == queryParameter.LibraryId,
            includeProperties: nameof(Movie.MediaFiles),
            orderBy: s => s.OrderBy(x => x.Title),
            offset: (queryParameter.PageNumber - 1) * queryParameter.PageSize,
            size: queryParameter.PageSize);
        return new ResultResource<Movie>
        {
            Items = items,
            TotalItems = totalItems
        };
    }

    [HttpGet("tvshow")]
    public async Task<ResultResource<TvShow>> GetTvShows([FromQuery] MediaQueryParameter queryParameter)
    {
        var totalItems = await _unitOfWork.TvShowRepository.Count(x => x.LibraryId == queryParameter.LibraryId);
        var items = await _unitOfWork.TvShowRepository.Get(x => x.LibraryId == queryParameter.LibraryId,
            s => s.OrderBy(x => x.Title),
            nameof(TvShow.Episodes),
            offset: (queryParameter.PageNumber - 1) * queryParameter.PageSize,
            size: queryParameter.PageSize);
        return new ResultResource<TvShow>
        {
            Items = items,
            TotalItems = totalItems
        };
    }

    [HttpGet("playlist")]
    public async Task<IEnumerable<PlaylistResource>> GetPlaylists()
    {
        var playlists = await _unitOfWork.PlaylistRepository.Get(includeProperties: nameof(Playlist.Server));
        List<PlaylistResource> playlistResources = new List<PlaylistResource>();
        foreach (var playlist in playlists)
        {
            var movies =
                (await _unitOfWork.MovieRepository.Get(x => playlist.Items.Contains(x.RatingKey),
                    orderBy: s => s.OrderBy(x => x.Title))).ToDictionary(x =>
                    x.RatingKey);
            var episodes =
                (await _unitOfWork.EpisodeRepository.Get(x => playlist.Items.Contains(x.RatingKey),
                    orderBy: s => s.OrderBy(x => x.Title),
                    includeProperties: nameof(Episode.TvShow))).ToDictionary(x => x.RatingKey);

            List<PlaylistItem> items = playlist.Items.Select(x =>
            {
                if (movies.TryGetValue(x, out Movie movie))
                    return new PlaylistItem()
                        { Id = x, Name = $"{movie.Title} ({movie.Year})", Type = ElementType.Movie };
                else if (episodes.TryGetValue(x, out Episode episode))
                    return new PlaylistItem()
                    {
                        Id = x,
                        Name =
                            $"{episode.Title} ({episode.TvShow.Title} S{episode.SeasonNumber}E{episode.EpisodeNumber})",
                        Type = ElementType.Movie
                    };
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

    [HttpGet("search")]
    public async Task<ActionResult<SearchResultResource>> Search([FromQuery] string searchTerm)
    {
        SearchResultResource result = new();

        var search = searchTerm.Trim('%').Insert(0, "%");
        search = search.Insert(search.Length, "%");
        var movies = (await _unitOfWork.MovieRepository.Get(x => EF.Functions.Like(x.Title, search),
            s => s.OrderBy(x => x.Title),
            includeProperties: nameof(Movie.MediaFiles))).ToList();
        result.Movies = movies.Take(100);
        result.TotalMoviesMatching = movies.Count();

        var tvshows = (await _unitOfWork.TvShowRepository.Get(x => EF.Functions.Like(x.Title, search),
            s => s.OrderBy(x => x.Title),
            nameof(TvShow.Episodes))).ToList();
        result.TvShows = tvshows.Take(10);
        result.TotalTvShowsMatching = tvshows.Count();

        // var episodes = (await _unitOfWork.EpisodeRepository.Get(x => EF.Functions.Like(x.Title, search))).ToList();
        // result.Episodes = episodes.Take(100);
        // result.TotalEpisodesMatching = episodes.Count();

        // var playlists = (await _unitOfWork.PlaylistRepository.Get(x => EF.Functions.Like(x.Name, search),
        //     includeProperties: nameof(Playlist.Server))).ToList();
        // result.Playlists = playlists.Take(100);
        // result.TotalPlaylistsMatching = playlists.Count();

        return result;
    }
}