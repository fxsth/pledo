using System.Diagnostics;
using System.Reflection;
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
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string? version = string.IsNullOrEmpty(assemblyLocation) ? "1.0.0" : FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
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
            Version = version
        };

        services
            .AddSingleton(apiOptions)
            .AddTransient<IPlexServerClient, PlexServerClient>()
            .AddTransient<IPlexAccountClient, PlexAccountClient>()
            .AddTransient<IPlexLibraryClient, CustomPlexLibraryClient>()
            .AddTransient<IApiService, ApiService>()
            .AddTransient<IPlexFactory, PlexFactory>()
            .AddSingleton<IPlexRequestsHttpClient, CustomPlexHttpClient>()
            .AddScoped<IApiService, ApiService>();
        return services;
    }
}