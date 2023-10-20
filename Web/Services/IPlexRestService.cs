using Plex.ServerApi.PlexModels.Account;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface IPlexRestService
{
    Task<PlexAccount?> LoginAccount(CredentialsResource credentialsResource);
    Task<Account?> GetMyPlexAccount(string authToken);
    Task<IEnumerable<Server>> RetrieveServers(Account account);
    Task<IEnumerable<Library>> RetrieveLibraries(Server server);
    Task<Movie?> RetrieveMovieByKey(Library library, string movieKey);
    Task<IEnumerable<Movie>> RetrieveMovies(Library library);
    Task<IEnumerable<TvShow>> RetrieveTvShows(Library library);
    Task<IEnumerable<Episode>> RetrieveEpisodes(Library library);
    Task<IEnumerable<Playlist>> RetrievePlaylists(Server library);
    Task<string?> GetTransientToken(Server server);
    IReadOnlyCollection<Uri> GetAllPossibleConnectionUrisForServer(Server server);
    Task<string> GetUriFromFastestConnection(Server server, int timeoutInSeconds);
}