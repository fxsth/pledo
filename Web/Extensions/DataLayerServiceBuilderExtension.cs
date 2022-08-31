using Plex.Api.Factories;
using Plex.Library.Factories;
using Plex.ServerApi;
using Plex.ServerApi.Api;
using Plex.ServerApi.Clients;
using Plex.ServerApi.Clients.Interfaces;
using Web.Data;
using Web.Models;
using Web.Services;

namespace Web.Extensions;

public static class DataLayerServiceBuilderExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {

        services
            .AddScoped<UnitOfWork>();
            // .AddScoped<IAccountRepository, AccountRepository>()
            // .AddScoped<IServerRepository, ServerRepository>()
            // .AddScoped<ILibraryRepository, LibraryRepository>()
            // .AddScoped<IMovieRepository, MovieRepository>();
        return services;
    }
}