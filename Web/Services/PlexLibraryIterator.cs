using System.Diagnostics;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Web.Models;
using Web.Models.Interfaces;

namespace Web.Services;

public class PlexLibraryIterator
{
    private readonly IPlexLibraryClient _plexLibraryClient;
    private readonly ILogger<PlexRestService> _logger;

    public PlexLibraryIterator(IPlexLibraryClient plexLibraryClient, ILogger<PlexRestService> logger)
    {
        _plexLibraryClient = plexLibraryClient;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<T>> GetWithDynamicBatchSize<T>(Library library, int maxBatchSize, TimeSpan minPauseBetweenRequests)
        where T : ISearchable
    {
        int totalSize = await GetTotalSize<T>(library);
        _logger.LogInformation("Library {0} contains {1} items. Start retrieving metadata...", library.Name, totalSize);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        
        int offset = 0;
        int batchSize = maxBatchSize;
        List<Task<IReadOnlyCollection<T>>> allRequests = new();
        while (offset < totalSize)
        {
            var elementRequest = GetWithDynamicBatchSize<T>(library, offset, batchSize);
            allRequests.Add(elementRequest);
            offset += batchSize;
            await Task.Delay(minPauseBetweenRequests);
        }
        
        List<T> allMediaElements = new List<T>();
        foreach (Task<IReadOnlyCollection<T>> request in allRequests)
        {
            try
            {
                var retrievedItems = await request;
                if(!retrievedItems.Any() && allMediaElements.Count < totalSize)
                    _logger.LogWarning("An error occured while retrieving metadata batch: Request returned 0 items in this batch. Sync will continue though.");
                allMediaElements.AddRange(retrievedItems);
            }
            catch (TaskCanceledException e)
            {
                _logger.LogError( "Cancelled metadata request: {0}", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unknown error occured while retrieving metadata batch.");
            }
        }

        stopwatch.Stop();
        _logger.LogDebug("Received {0} items metadata from library {1}, duration: {2} ms",
            allMediaElements.Count, library.Name, stopwatch.ElapsedMilliseconds);

        return allMediaElements;
    }
    
    private async Task<IReadOnlyCollection<T>> GetWithDynamicBatchSize<T>(Library library, int offset, int maxBatchSize)
        where T : ISearchable
    {
        var elementsRequest = Get<T>(library, offset, maxBatchSize);
        var collection = await elementsRequest;
        if (collection.Count == 0)
        {
            if (maxBatchSize <= 1) return collection;
            _logger.LogDebug("Request returned 0 items in this batch. Sync will try again with smaller batch size.");
            var firstBatchSize = maxBatchSize / 2;
            var secondBatchSize = maxBatchSize - firstBatchSize;
            var collection1 = await GetWithDynamicBatchSize<T>(library, offset, firstBatchSize);
            var collection2 = await GetWithDynamicBatchSize<T>(library, offset + firstBatchSize, secondBatchSize);
            var list = new List<T>(collection1);
            list.AddRange(collection2);
            return list;
        }

        return collection;
    }

    private async Task<IReadOnlyCollection<T>> Get<T>(Library library, int offset, int batchSize)
        where T : ISearchable
    {
        var searchType = MapToSearchType<T>();
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, searchType, null, offset, batchSize);
        if (mediaContainer.Media == null)
            return new List<T>();
        return Mapper.GetFromMediaContainer<T>(mediaContainer, library, _logger).ToList();
    }
    
    private async Task<int> GetTotalSize<T>(Library library)
        where T : ISearchable
    {
        var searchType = MapToSearchType<T>();
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, searchType, null, 0, 0);
        return mediaContainer.TotalSize;
    }

    private SearchType MapToSearchType<T>() where T : ISearchable
    {
        if (typeof(T) == typeof(Movie))
            return SearchType.Movie;
        if (typeof(T) == typeof(Episode))
            return SearchType.Episode;
        if (typeof(T) == typeof(TvShow))
            return SearchType.Show;
        throw new Exception("Could not map type to search type.");
    }
}