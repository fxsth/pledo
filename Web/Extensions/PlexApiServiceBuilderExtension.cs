using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
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
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        ClientOptions apiOptions = new ClientOptions
        {
            Product = "pledo",
            DeviceName = Environment.MachineName,
            ClientId = PreferencesProvider.GetClientId(),
            Platform = RuntimeInformation.OSDescription,
            Version = fileVersionInfo.FileVersion
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