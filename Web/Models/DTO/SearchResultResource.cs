namespace Web.Models.DTO;

public class SearchResultResource
{
    public int TotalMoviesMatching { get; set; }
    public IEnumerable<Movie> Movies { get; set; }
    public int TotalTvShowsMatching { get; set; }
    public IEnumerable<TvShow> TvShows { get; set; }
    public int TotalEpisodesMatching { get; set; }
    public IEnumerable<Episode> Episodes { get; set; }
    public int TotalPlaylistsMatching { get; set; }
    public IEnumerable<PlaylistResource> Playlists { get; set; }
}