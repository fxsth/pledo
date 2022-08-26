using System.Collections.ObjectModel;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Web.Data;
using Web.Models;

namespace Web.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly HttpClient _httpClient;
        private readonly DbContext _dbContext;
        private readonly ClientOptions _clientOptions;

        public DownloadService(HttpClient httpClient, DbContext dbContext, ClientOptions clientOptions)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
            _clientOptions = clientOptions;
            PendingDownloads = new Collection<DownloadElement>();
        }

        private bool _isDownloading;

        public event EventHandler<DownloadElement>? TaskStarted;
        public event EventHandler? AllTasksFinished;

        public bool IsRunning => _isDownloading;
        public string TaskName => "File Download";

        public Collection<DownloadElement> PendingDownloads { get; }

        public async Task Download(string key)
        {
            Movie? movie = await _dbContext.Movies.FindAsync(key);
            if (movie == null)
                throw new ArgumentException();
            var server = await _dbContext.Servers.FindAsync(movie.ServerId);
            if (server == null)
                throw new ArgumentException();
            ApiRequestBuilder apiRequestBuilder =
                new ApiRequestBuilder(server.LastKnownUri, movie.DownloadUri, HttpMethod.Get)
                    .AddPlexToken(server.AccessToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(_clientOptions.ClientId));
            var apiRequest = apiRequestBuilder.Build();
            AddToPendingDownloads(
                new DownloadElement()
                {
                    Name = movie.Title,
                    Uri = apiRequest.FullUri,
                    ElementType = ElementType.Movie,
                    FilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                });
        }

        private void AddToPendingDownloads(IEnumerable<DownloadElement> toDownload)
        {
            foreach (DownloadElement element in toDownload)
                AddToPendingDownloads(element);
        }

        private void AddToPendingDownloads(DownloadElement toDownload)
        {
            if (!PendingDownloads.Any(x => x.Id == toDownload.Id))
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
        }

        private async Task Postprocess(DownloadElement downloadElement)
        {
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
                    await CopyToAsync(response, fileStream, downloadElement.Progress,
                        downloadElement.CancellationTokenSource.Token);
                }

                downloadElement.FinishedSuccessfully = true;
            }
            catch (Exception e)
            {
            }
        }

        private static async Task CopyToAsync(Stream source, Stream destination, IProgress<double> progress,
            CancellationToken cancellationToken = default(CancellationToken), int bufferSize = 0x1000)
        {
            var buffer = new byte[bufferSize];
            int bytesRead;
            // DownloadProgress downloadProgress = new DownloadProgress() { Total = source.Length, Downloaded = 0 };
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                //downloadProgress.Downloaded += bytesRead;
                //progress.Report(downloadProgress);
            }
        }
    }
}