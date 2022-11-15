using Web.Data;

namespace Web.Extensions;

public static class DataLayerServiceBuilderExtension
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services)
    {
        services.AddScoped<UnitOfWork>();
        return services;
    }
}