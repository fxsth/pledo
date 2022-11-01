using System.Collections.ObjectModel;
using Web.Data;
using Web.Models;

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
                if (elementType == ElementType.Movie)
                {
                    Movie? movie = unitOfWork.MovieRepository.Get(x => x.RatingKey == key).FirstOrDefault();
                    if (movie == null)
                        throw new ArgumentException();
                    var movieDirectory = await settingsService.GetMovieDirectory();
                    Directory.CreateDirectory(movieDirectory);
                    return new DownloadElement()
                    {
                        Uri = (await GetCompleteDownloadUri(unitOfWork, movie.LibraryId, movie.DownloadUri)).ToString(),
                        Name = movie.Title,
                        ElementType = elementType,
                        FilePath = Path.Combine(movieDirectory, Path.GetFileName(movie.ServerFilePath)),
                        FileName = Path.GetFileName(movie.ServerFilePath),
                        TotalBytes = movie.TotalBytes
                    };
                }
                else
                {
                    Episode? episode = unitOfWork.EpisodeRepository.Get(x => x.RatingKey == key).FirstOrDefault();
                    if (episode == null)
                        throw new ArgumentException();
                    var episodeDirectory = await settingsService.GetEpisodeDirectory();
                    Directory.CreateDirectory(episodeDirectory);
                    return new DownloadElement()
                    {
                        Uri = (await GetCompleteDownloadUri(unitOfWork, episode.LibraryId, episode.DownloadUri))
                            .ToString(),
                        Name = episode.Title,
                        ElementType = elementType,
                        FilePath = Path.Combine(episodeDirectory, Path.GetFileName(episode.ServerFilePath)),
                        FileName = Path.GetFileName(episode.ServerFilePath),
                        TotalBytes = episode.TotalBytes
                    };
                }
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

        public Task DownloadSeries(string key)
        {
            throw new NotImplementedException();
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
                // downloadElement.CancellationTokenSource.CancelAfter(60000);
                Stream response = await _httpClient.GetStreamAsync(downloadElement.Uri,
                    downloadElement.CancellationTokenSource.Token);

                var fileInfo = new FileInfo(downloadElement.FilePath);
                fileInfo.Directory.Create();
                using (var fileStream = fileInfo.OpenWrite())
                {
                    await CopyToAsync(response, fileStream, downloadElement);
                }

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