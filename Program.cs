using karaoke_place.Data;
using karaoke_place.Api.Common;
using karaoke_place.Modules.Diagnostic;
using karaoke_place.Modules.KaraokeEvents;
using karaoke_place.Modules.Users;
using karaoke_place.Modules.Songs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Microsoft.OpenApi;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Karaoke Place API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter a valid Auth0 bearer token."
    });
});
builder.Services.AddOpenApi(options =>
{
    // ASP.NET Core OpenAPI (Microsoft.OpenApi v2) emits type as a JsonSchemaType
    // flags enum. For int properties it sets Integer|String, which serialises as
    // ["integer","string"]. Strip the String flag so ids resolve as number only.
    options.AddSchemaTransformer((schema, context, cancellationToken) =>
    {
        if (schema.Type.HasValue
            && schema.Type.Value.HasFlag(JsonSchemaType.Integer)
            && schema.Type.Value.HasFlag(JsonSchemaType.String))
        {
            schema.Type = schema.Type.Value & ~JsonSchemaType.String;
            schema.Pattern = null;
        }

        return Task.CompletedTask;
    });
});

// Drive required vs optional directly from Nullable Reference Type annotations:
// non-nullable properties (string, int, …) → required; nullable (string?, int?) → optional.
builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.RespectNullableAnnotations = true;
    options.JsonSerializerOptions.RespectRequiredConstructorParameters = true;
});

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
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Karaoke Place API v1");
    options.RoutePrefix = "swagger";
});
app.UseAuthentication();
app.UseAuthorization();
app.MapOpenApi();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
