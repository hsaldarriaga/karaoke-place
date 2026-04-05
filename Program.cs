using karaoke_place.Data;
using karaoke_place.Api.Diagnostic;
using karaoke_place.Modules.Diagnostic;
using karaoke_place.Modules.KaraokeEvents;
using karaoke_place.Modules.Auth;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Songs;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOpenApi();
builder.Services.AddScoped<DiagnosticService>();
builder.Services.AddScoped<KaraokeEventRepository>();
builder.Services.AddScoped<KaraokeEventService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SongRepository>();
builder.Services.AddScoped<SongService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserContext>();
builder.Services.AddScoped<RequireMockAuthFilter>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapOpenApi();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
