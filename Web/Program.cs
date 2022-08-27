using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Extensions;
using Web.Services;
using DbContext = Web.Data.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPlexServices()
    .AddDataLayer()
    .AddScoped<ISettingsService, SettingsService>()
    .AddScoped<IPlexService, PlexService>()
    .AddSingleton<HttpClient>()
    .AddSingleton<IDownloadService, DownloadService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<DbContext>(o =>
    {
        o.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
        // o.UseInMemoryDatabase(builder.Configuration.GetConnectionString("Database"));
        o.EnableSensitiveDataLogging(true);
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

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DbContext>();
    // context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
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