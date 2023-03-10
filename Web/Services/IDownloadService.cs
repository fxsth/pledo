﻿using Web.Models;

namespace Web.Services;

public interface IDownloadService
{
    IReadOnlyCollection<DownloadElement> GetPendingDownloads();
    IReadOnlyCollection<DownloadElement> GetAll();
    Task DownloadMovie(string key);
    Task DownloadEpisode(string key);
    Task DownloadSeason(string key, int season);
    Task DownloadTvShow(string key);
    Task DownloadPlaylist(string key);
    Task CancelDownload(string key);

}