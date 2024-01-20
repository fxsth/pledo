using Web.Models;

namespace Web.Services;

public interface IDownloadService
{
    IReadOnlyCollection<DownloadElement> GetPendingDownloads();
    IReadOnlyCollection<DownloadElement> GetAll();
    Task RemoveAllFinishedOrCancelledDownloads();
    Task DownloadMovie(string key, string mediaFileKey);
    Task DownloadEpisode(string key, string mediaFileKey);
    Task DownloadSeason(string key, int season);
    Task DownloadTvShow(string key);
    Task DownloadPlaylist(string key);
    Task CancelDownload(string key);

    event Action<int> PendingDownloadCountChanged;

}