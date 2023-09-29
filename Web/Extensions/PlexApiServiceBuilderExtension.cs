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
        bool runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        string osName = OperatingSystem.IsWindows() ? "Windows" :
            OperatingSystem.IsLinux() ? "Linux" :
            OperatingSystem.IsMacOS() ? "MacOS" : "Unknown";
        string deviceName = runningInContainer ? $"{osName} Container" : $"{osName} Machine";
            ClientOptions apiOptions = new ClientOptions
        {
            Product = "pledo",
            DeviceName = deviceName,
            ClientId = PreferencesProvider.GetClientId(),
            Platform = osName,
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