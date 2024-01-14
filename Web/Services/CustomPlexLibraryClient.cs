﻿using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels;
using Plex.ServerApi.PlexModels.Folders;
using Plex.ServerApi.PlexModels.Hubs;
using Plex.ServerApi.PlexModels.Library.Collections;
using Plex.ServerApi.PlexModels.Library.Search;
using Plex.ServerApi.PlexModels.Library.Search.Plex.Api.PlexModels.Library.Search;
using Plex.ServerApi.PlexModels.Media;
using Plex.ServerApi.PlexModels.PlayQueues;

namespace Web.Services
{
    /// <summary>
    ///
    /// </summary>
    public class CustomPlexLibraryClient : IPlexLibraryClient
    {
        private readonly IApiService apiService;
        private readonly ClientOptions clientOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlexLibraryClient"/> class.
        /// </summary>
        /// <param name="clientOptions">Plex Client Options.</param>
        /// <param name="apiService">Api Service.</param>
        public CustomPlexLibraryClient(ClientOptions clientOptions, IApiService apiService)
        {
            this.apiService = apiService;
            this.clientOptions = clientOptions;
        }

        /// <inheritdoc/>
        public async Task EmptyTrash(string authToken, string plexServerHost, string key)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/sections/{key}/emptyTrash", HttpMethod.Put)
                    .AddPlexToken(authToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task ScanForNewItems(string authToken, string plexServerHost, string key,
            bool forceMetadataRefresh)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/sections/{key}/refresh", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParam("force", forceMetadataRefresh ? "1" : "0")
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task CancelScanForNewItems(string authToken, string plexServerHost, string key)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/sections/{key}/refresh", HttpMethod.Delete)
                    .AddPlexToken(authToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task<FilterFieldContainer> GetFilterFields(string authToken, string plexServerHost, string key)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "includeMeta", "1" }, { "X-Plex-Container-Start", "0" }, { "X-Plex-Container-Size", "0" }
            };

            var items = await this.FetchWithWrapper<FilterFieldContainer>(plexServerHost,
                $"library/sections/{key}/all", authToken,
                HttpMethod.Get, queryParams);

            return items;
        }

        /// <inheritdoc/>
        public async Task<HubMediaContainer> HubLibrarySearch(string authToken, string plexServerHost, string title)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "query", title },
                { "includeCollections", "1" },
                { "includeExternalMedia", "1" },
                { "includePreferences", "1" },
                { "includeExtras", "1" },
                { "includeStations", "1" },
                { "includeChapters", "1" },
                { "includeGuids", "1" }
            };

            return await this.FetchWithWrapper<HubMediaContainer>(plexServerHost, "hubs/search", authToken,
                HttpMethod.Get, queryParams);
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> LibrarySearch(string authToken, string plexServerHost, string title,
            string libraryKey, string sort, SearchType libraryType, List<FilterRequest> filters = null, int start = 0,
            int count = 100)
        {
            var queryParams = new Dictionary<string, string> { { "type", ((int)libraryType).ToString() } };

            if (!string.IsNullOrEmpty(title))
            {
                queryParams.Add("title", title);
            }

            if (!string.IsNullOrEmpty(sort))
            {
                queryParams.Add("sort", sort);
            }

            // Include Guids now available PMS v1.24.3.5033
            queryParams.Add("includeGuids", "1");

            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/sections/{libraryKey}/all", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(queryParams)
                    .AddFilterFields(filters)
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AddQueryParams(ClientUtilities.GetClientLimitHeaders(start, count))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<MediaContainer>>(apiRequest);
            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> GetItem(string authToken, string plexServerHost, string key)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/metadata/{key}", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(ClientUtilities.GetLibraryFlagFields())
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<MediaContainer>>(apiRequest);

            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> GetExtras(string authToken, string plexServerHost, string key)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/metadata/{key}/extras", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(ClientUtilities.GetLibraryFlagFields())
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<MediaContainer>>(apiRequest);

            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task<int> GetLibrarySize(string authToken, string plexServerHost, string key)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/sections/{key}/all", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<MediaContainer>>(apiRequest);

            return wrapper.Container.Size;
        }

        /// <inheritdoc/>
        public async Task<FolderContainer> GetLibraryFolders(string authToken, string plexServerHost, string key) =>
            await this.FetchWithWrapper<FolderContainer>(plexServerHost, $"library/sections/{key}/folder", authToken,
                HttpMethod.Get);

        /// <inheritdoc/>
        public async Task<FilterValueContainer> GetLibraryFilterValues(string authToken, string plexServerHost,
            string key, string filterUri) =>
            await this.FetchWithWrapper<FilterValueContainer>(plexServerHost,
                filterUri, authToken,
                HttpMethod.Get);

        /// <inheritdoc/>
        public async Task<FilterContainer>
            GetLibraryFilters(string authToken, string plexServerHost, string librarykey) =>
            await this.FetchWithWrapper<FilterContainer>(plexServerHost, $"library/sections/{librarykey}/filters",
                authToken,
                HttpMethod.Get);

        /// <inheritdoc/>
        public async Task<CollectionContainer> GetCollectionsAsync(string authToken, string plexServerHost,
            string libraryKey, string title)
        {
            var queryParams =
                new Dictionary<string, string> { { "includeCollections", "1" }, { "includeExternalMedia", "true" } };

            if (!string.IsNullOrEmpty(title))
            {
                queryParams.Add("title" + "%3d", title);
            }

            var apiRequest = new ApiRequestBuilder(plexServerHost, $"library/sections/{libraryKey}/collections",
                    HttpMethod.Get)
                .AddQueryParams(queryParams)
                .AddPlexToken(authToken)
                .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                .AcceptJson()
                .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<CollectionContainer>>(apiRequest);

            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task UpdateCollectionAsync(string authToken, string plexServerHost, string libraryKey,
            CollectionModel collectionModel)
        {
            if (authToken == null)
            {
                throw new ArgumentNullException(nameof(authToken));
            }

            if (plexServerHost == null)
            {
                throw new ArgumentNullException(nameof(plexServerHost));
            }

            if (libraryKey == null)
            {
                throw new ArgumentNullException(nameof(libraryKey));
            }

            if (collectionModel == null)
            {
                throw new ArgumentNullException(nameof(collectionModel));
            }

            var apiRequest =
                new ApiRequestBuilder(plexServerHost, "library/sections/" + libraryKey + "/all", HttpMethod.Put)
                    .AddPlexToken(authToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .AddQueryParams(new Dictionary<string, string>()
                    {
                        { "type", "18" },
                        { "id", collectionModel.RatingKey },
                        { "includeExternalMedia", "1" },
                        { "title.value", collectionModel.Title },
                        { "titleSort.value", collectionModel.TitleSort },
                        { "summary.value", collectionModel.Summary },
                        { "contentRating.value", collectionModel.ContentRating },
                        { "title.locked", "1" },
                        { "titleSort.locked", "1" },
                        { "contentRating.locked", "1" },
                    })
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task<CollectionContainer> GetCollectionAsync(string authToken, string plexServerHost,
            string collectionKey)
        {
            var queryParams =
                new Dictionary<string, string> { { "includeCollections", "1" }, { "includeExternalMedia", "true" } };

            var apiRequest =
                new ApiRequestBuilder(plexServerHost, "library/collections/" + collectionKey, HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(queryParams)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<CollectionContainer>>(apiRequest);

            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> GetCollectionItemsAsync(string authToken, string plexServerHost,
            string collectionKey)
        {
            var queryParams =
                new Dictionary<string, string>
                {
                    { "includeCollections", "1" },
                    { "includeExternalMedia", "1" },
                    { "includeExtras", "1" },
                    { "includeGuids", "1" },
                    { "skipRefresh", "1" },
                    { "includePreferences", "1" },
                    { "includeStations", "1" },
                    { "includeChapters", "1" },
                };

            return await this.FetchWithWrapper<MediaContainer>(plexServerHost,
                "library/metadata/" + collectionKey + "/children",
                authToken, HttpMethod.Get, queryParams);
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> GetCollectionItemsByCollectionName(string authToken, string plexServerHost,
            string libraryKey, string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentNullException(nameof(collectionName));
            }

            if (string.IsNullOrEmpty(libraryKey))
            {
                throw new ArgumentNullException(nameof(libraryKey));
            }

            var collection = await this.GetCollectionsAsync(authToken, plexServerHost, libraryKey, collectionName);
            if (collection.Size == 0)
            {
                return null;
            }

            return await this.GetCollectionItemsAsync(authToken, plexServerHost,
                collection.Collections.First().RatingKey);
        }

        /// <inheritdoc/>
        public async Task<MediaContainer> GetCollectionItemMetadataByKey(string authToken, string plexServerHost,
            string collectionKey)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"library/collections/{collectionKey}/children", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<MediaContainer>>(apiRequest);
            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task<PlayQueueContainer> CreatePlayQueue(string authToken, string plexServerHost, string hostIdentifier, string type,
            string key, bool isRepeat, bool isShuffle, bool isContinous)
        {
            var uri = "server://" + hostIdentifier + "/com.plexapp.plugins.library/library/metadata/" + key;

            var queryParams =
                new Dictionary<string, string>
                {
                    { "continuous", isContinous ? "1" : "0" },
                    { "uri", uri },
                    { "type", type },
                    { "repeat", isRepeat ? "1" : "0" },
                    { "shuffle", isShuffle ? "1" : "0" },
                    { "own", "1" },
                    { "includeChapters", "1" },
                    { "includeMarkers", "1" },
                    { "includeGeolocation", "1" },
                    { "includeExternalMedia", "1" },
                };

            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"playQueues", HttpMethod.Post)
                    .AddPlexToken(authToken)
                    .AddQueryParams(queryParams)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<PlayQueueContainer>>(apiRequest);
            return wrapper.Container;
        }

        /// <inheritdoc/>
        public async Task SendPlayQueueToPlayer(string plexServerHost, string authToken, string hostIdentifier, PlayQueueContainer playQueue, string type, string playerIdentifier, string transientToken, long offset)
        {
            var uri = new Uri(plexServerHost);

            var queryParams =
                new Dictionary<string, string>
                {
                    { "providerIdentifier", "com.plexapp.plugins.library"},
                    { "protocol", uri.Scheme},
                    { "address", uri.DnsSafeHost },
                    { "port", uri.Port.ToString() },
                    { "containerKey", "/playQueues/" + playQueue.PlayQueueId + "?own=1" },
                    { "key", "/library/metadata/" + playQueue.PlayQueueSelectedMetadataItemId },
                    { "offset", offset.ToString() },
                    { "commandID", "1" },
                    { "type", type },
                    { "machineIdentifier", hostIdentifier },
                    { "token", transientToken }
                };

            var apiRequest =
                new ApiRequestBuilder(plexServerHost, $"player/playback/playMedia", HttpMethod.Get)
                    .AddPlexToken(authToken)
                    .AddQueryParams(queryParams)
                    .AddHeader("X-Plex-Target-Client-Identifier", playerIdentifier)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task AddCollectionToLibraryItemAsync(string authToken, string plexServerHost, string libraryKey,
            string ratingKey, string collectionName)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, "library/sections/" + libraryKey + "/all", HttpMethod.Put)
                    .AddPlexToken(authToken)
                    .AddRequestHeaders(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .AddQueryParams(new Dictionary<string, string>()
                    {
                        { "type", "1" },
                        { "id", ratingKey },
                        { "includeExternalMedia", "1" },
                        { "collection[0].tag.tag", collectionName },
                        { "collection.locked", "1" },
                    })
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <inheritdoc/>
        public async Task DeleteCollectionFromLibraryItemAsync(string authToken, string plexServerHost,
            string libraryKey, string ratingKey, string collectionName)
        {
            var apiRequest =
                new ApiRequestBuilder(plexServerHost, "library/sections/" + libraryKey + "/all", HttpMethod.Put)
                    .AddPlexToken(authToken)
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .AddJsonBody(new Dictionary<string, string>()
                    {
                        { "type", "1" },
                        { "id", ratingKey },
                        { "includeExternalMedia", "1" },
                        { "collection.locked", "1" },
                        { "collection[0].tag.tag-", collectionName },
                    })
                    .Build();

            await this.apiService.InvokeApiAsync(apiRequest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="endpoint"></param>
        /// <param name="authToken"></param>
        /// <param name="method"></param>
        /// <param name="queryParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<T> FetchWithWrapper<T>(string baseUrl, string endpoint, string authToken, HttpMethod method,
            Dictionary<string, string> queryParams = null)
        {
            var apiRequest =
                new ApiRequestBuilder(baseUrl, endpoint, method)
                    .AddPlexToken(authToken)
                    .AddQueryParams(queryParams)
                    .AddQueryParams(ClientUtilities.GetClientIdentifierHeader(this.clientOptions.ClientId))
                    .AcceptJson()
                    .Build();

            var wrapper = await this.apiService.InvokeApiAsync<GenericWrapper<T>>(apiRequest);

            return wrapper.Container;
        }
    }
}