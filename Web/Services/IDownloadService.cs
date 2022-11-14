using System.Collections.ObjectModel;
using Web.Models;

namespace Web.Services;

public interface IDownloadService
{
    Collection<DownloadElement> PendingDownloads { get; }
    Task DownloadMovie(string key);
    Task DownloadEpisode(string key);
    Task DownloadSeason(string key, int season);

}