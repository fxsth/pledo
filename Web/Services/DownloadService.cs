using System.Collections.ObjectModel;
using OctaneEngine;
using Web.Data;
using Web.Models;
using Web.Models.Interfaces;

namespace Web.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceScopeFactory _scopeFactory;

        // private readonly ILogger _logger;

        public DownloadService(HttpClient httpClient, IServiceScopeFactory scopeFactory)
        {
            _httpClient = httpClient;
            _scopeFactory = scopeFactory;
            // _logger = logger;
            PendingDownloads = new Collection<DownloadElement>();
        }

        private bool _isDownloading;

        public event EventHandler<DownloadElement>? TaskStarted;
        public event EventHandler? AllTasksFinished;

        public bool IsRunning => _isDownloading;
        public string TaskName => "File Download";

        public Collection<DownloadElement> PendingDownloads { get; }

        private async Task<DownloadElement> CreateDownloadElement(string key, ElementType elementType)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                ISettingsService settingsService = scope.ServiceProvider.GetRequiredService<ISettingsService>();
                IMediaElement? mediaElement = await GetMediaElement(unitOfWork, elementType, key);
                if (mediaElement == null)
                    throw new ArgumentException();
                var downloadDirectory = elementType == ElementType.Movie
                    ? await settingsService.GetMovieDirectory()
                    : await settingsService.GetEpisodeDirectory();
                Directory.CreateDirectory(downloadDirectory);
                return new DownloadElement()
                {
                    Uri = (await GetCompleteDownloadUri(unitOfWork, mediaElement.LibraryId, mediaElement.DownloadUri))
                        .ToString(),
                    Name = mediaElement.Title,
                    ElementType = elementType,
                    FilePath = Path.Combine(downloadDirectory, Path.GetFileName(mediaElement.ServerFilePath)),
                    FileName = Path.GetFileName(mediaElement.ServerFilePath),
                    TotalBytes = mediaElement.TotalBytes
                };
            }
        }

        private async Task<Uri> GetCompleteDownloadUri(UnitOfWork unitOfWork, string libraryId,
            string resourceDownloadUri)
        {
            Library? library = unitOfWork.LibraryRepository.Get(x => x.Id == libraryId, null, nameof(Library.Server))
                .FirstOrDefault();
            if (library == null || library.Server?.LastKnownUri == null)
                throw new ArgumentException();
            UriBuilder uriBuilder = new UriBuilder(library.Server.LastKnownUri)
            {
                Path = resourceDownloadUri,
                Query = $"?X-Plex-Token={library.Server.AccessToken}"
            };
            return uriBuilder.Uri;
        }

        private async Task<IMediaElement?> GetMediaElement(UnitOfWork unitOfWork, ElementType elementType, string key)
        {
            if (elementType == ElementType.Movie)
            {
                return unitOfWork.MovieRepository.Get(x => x.RatingKey == key).FirstOrDefault();
            }
            else if (elementType == ElementType.TvShow)
            {
                return unitOfWork.EpisodeRepository.Get(x => x.RatingKey == key).FirstOrDefault();
            }

            return null;
        }

        public async Task DownloadMovie(string key)
        {
            var downloadElement = await CreateDownloadElement(key, ElementType.Movie);
            AddToPendingDownloads(downloadElement);
        }

        public async Task DownloadEpisode(string key)
        {
            var downloadElement = await CreateDownloadElement(key, ElementType.TvShow);
            AddToPendingDownloads(downloadElement);
        }

        public async Task DownloadSeason(string key, int season)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                TvShow? tvShow = unitOfWork.TvShowRepository.Get(x => x.RatingKey == key).FirstOrDefault();
                if (tvShow == null)
                    throw new InvalidOperationException();
                var episodes = tvShow.Episodes.Where(x => x.SeasonNumber == season);
                foreach (Episode episode in episodes)
                {
                    var downloadElement = await CreateDownloadElement(episode.Key, ElementType.TvShow);
                    AddToPendingDownloads(downloadElement);
                }
            }
        }

        private void AddToPendingDownloads(IEnumerable<DownloadElement> toDownload)
        {
            foreach (DownloadElement element in toDownload)
                AddToPendingDownloads(element);
        }

        private void AddToPendingDownloads(DownloadElement toDownload)
        {
            if (PendingDownloads.All(x => x.Id != toDownload.Id))
            {
                PendingDownloads.Add(toDownload);
                StartDownloaderIfNotActive();
            }
        }

        private void StartDownloaderIfNotActive()
        {
            if (!_isDownloading)
            {
                _isDownloading = true;
                Task.Run(async () => await DownloadQueue());
            }
        }

        private async Task DownloadQueue()
        {
            while (PendingDownloads.Count > 0)
            {
                _isDownloading = true;
                DownloadElement downloadElement = PendingDownloads.First();
                TaskStarted?.Invoke(this, downloadElement);

                await Preprocess(downloadElement);
                await DownloadFile(downloadElement);
                await Postprocess(downloadElement);

                PendingDownloads.RemoveAt(0);
            }

            _isDownloading = false;
            AllTasksFinished?.Invoke(this, new EventArgs());
        }

        private async Task Preprocess(DownloadElement downloadElement)
        {
            downloadElement.Progress = new Progress<double>();
            downloadElement.Started = DateTimeOffset.Now;
        }

        private async Task Postprocess(DownloadElement downloadElement)
        {
            downloadElement.Finished = DateTimeOffset.Now;
        }

        private async Task DownloadFile(DownloadElement downloadElement)
        {
            try
            {
                // // downloadElement.CancellationTokenSource.CancelAfter(60000);
                // Stream response = await _httpClient.GetStreamAsync(downloadElement.Uri,
                //     downloadElement.CancellationTokenSource.Token);
                //
                // var fileInfo = new FileInfo(downloadElement.FilePath);
                // fileInfo.Directory.Create();
                // using (var fileStream = fileInfo.OpenWrite())
                // {
                //     await CopyToAsync(response, fileStream, downloadElement);
                // }
                OctaneConfiguration octaneConfiguration = new OctaneConfiguration() { Parts = 1,ProgressCallback = d =>
                    {
                        downloadElement.Progress.Report(d);
                        downloadElement.DownloadedBytes = Convert.ToInt64(downloadElement.TotalBytes * d);
                    }
                };
                await Engine.DownloadFile(downloadElement.Uri, outFile: downloadElement.FilePath, octaneConfiguration);

                downloadElement.FinishedSuccessfully = true;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private static async Task CopyToAsync(Stream source, Stream destination, DownloadElement downloadElement,
            int bufferSize = 0x1000)
        {
            CancellationToken cancellationToken = downloadElement.CancellationTokenSource.Token;
            var buffer = new byte[bufferSize];
            int bytesRead;
            // DownloadProgress downloadProgress = new DownloadProgress() { Total = source.Length, Downloaded = 0 };
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                Console.WriteLine(
                    $"Download progress: {downloadElement.DownloadedBytes * 100 / downloadElement.TotalBytes}% - {downloadElement.DownloadedBytes}/{downloadElement.TotalBytes}");
                downloadElement.DownloadedBytes += bytesRead;
            }
        }
    }
}