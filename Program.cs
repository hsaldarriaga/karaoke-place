using karaoke_place.Data;
using karaoke_place.Api.Common;
using karaoke_place.Modules.Diagnostic;
using karaoke_place.Modules.KaraokeEvents;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Songs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
var allowedOriginsValue = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsValue)
    ? null
    : allowedOriginsValue.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

var auth0DomainValue = Environment.GetEnvironmentVariable("AUTH0_DOMAIN");
var auth0Domain = string.IsNullOrWhiteSpace(auth0DomainValue)
    ? null
    : auth0DomainValue.StartsWith("http", StringComparison.OrdinalIgnoreCase)
        ? auth0DomainValue.TrimEnd('/')
        : $"https://{auth0DomainValue.TrimEnd('/')}";

var auth0Audience = Environment.GetEnvironmentVariable("AUTH0_AUDIENCE");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

if (allowedOrigins != null) {
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Frontend", policy =>
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        if (!string.IsNullOrWhiteSpace(auth0Domain))
            options.Authority = auth0Domain;

        if (!string.IsNullOrWhiteSpace(auth0Audience))
            options.Audience = auth0Audience;
    });

builder.Services.AddAuthorization();
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
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
