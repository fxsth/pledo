using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using Web.Services;

namespace Web.Extensions;

public static class LoggingFilterServiceBuilderExtension
{
    public static void AddLogFilter(this WebApplicationBuilder builder)
    {
        builder.Logging.AddFilter((provider, category, logLevel) =>
        {
            if (provider == "ApiService" && category.Contains("Plex") && logLevel >= LogLevel.Warning)
                return true;
            return false;
        });
    }
}