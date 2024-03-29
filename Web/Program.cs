using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web.Data;
using Web.Extensions;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPlexServices()
    .AddDataLayer()
    .AddScoped<ISettingsService, SettingsService>()
    .AddScoped<ILoginService, LoginService>()
    .AddSingleton<ISyncService, SyncService>()
    .AddHttpClient()
    .AddScoped<IPlexRestService, PlexRestService>()
    .AddScoped<PlexLibraryIterator>()
    .AddSingleton<HttpClient>()
    .AddSingleton<IDownloadService, DownloadService>()
    .AddHostedService<PeriodicallySyncBackgroundService>();

builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddDbContext<CustomDbContext>(o =>
    {
        // o.UseSqlServer(builder.Configuration.GetConnectionString("LocalDbDatabase"));
        // o.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database"));
        var connectionString = builder.Configuration.GetConnectionString("SqliteDatabase");
        string dataDirectory = PreferencesProvider.GetDataDirectory();
        if(dataDirectory.Any() && !dataDirectory.EndsWith(Path.DirectorySeparatorChar))
            dataDirectory += Path.DirectorySeparatorChar;
        connectionString = connectionString.Replace("|DataDirectory|", dataDirectory);
        o.UseSqlite(connectionString);
        // o.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
#if DEBUG
        o.EnableDetailedErrors();
        o.EnableSensitiveDataLogging();
#endif
    }
);
builder.Services.AddSwaggerGen();
builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#endif

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CustomDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception e)
    {
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }

    DbInitializer.Initialize(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "api/{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();