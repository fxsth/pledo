using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using Web.Services;

namespace Web.Extensions;

public static class PlexApiServiceBuilderExtension
{
    public static IServiceCollection AddPlexServices(this IServiceCollection services)
    {
        ClientOptions apiOptions = new ClientOptions
        {
            Product = "pledo",
            DeviceName = Environment.MachineName,
            ClientId = "ff0106f8-6106-4983-8099-b441ce5dbf2c",
            Platform = "Windows",
            Version = "v1"
        };

        services
            .AddSingleton(apiOptions)
            .AddTransient<IPlexServerClient, PlexServerClient>()
            .AddTransient<IPlexAccountClient, PlexAccountClient>()
            .AddTransient<IPlexLibraryClient, PlexLibraryClient>()
            .AddTransient<IApiService, ApiService>()
            .AddTransient<IPlexFactory, PlexFactory>()
            .AddSingleton<IPlexRequestsHttpClient, CustomPlexHttpClient>()
            .AddScoped<IApiService, ApiService>();
        return services;
    }
}