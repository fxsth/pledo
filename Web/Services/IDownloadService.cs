using System.Collections.ObjectModel;
using Web.Models;

namespace Web.Services;

public interface IDownloadService
{
    Collection<DownloadElement> PendingDownloads { get; }
    Task Download(string key);

}