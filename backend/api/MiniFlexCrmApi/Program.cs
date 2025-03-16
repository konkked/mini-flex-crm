using System.Text.Json;
using MiniFlexCrmApi.Auth;
using MiniFlexCrmApi.Cache;
using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Db.Repos;
using MiniFlexCrmApi.Serialization;
using MiniFlexCrmApi.Services;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

string GetConnectionString(IServiceProvider services)
{
    var connectionString =
        services.GetService<IConfiguration>()?.GetConnectionString("DefaultConnection") ?? string.Empty;
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
    }
    return connectionString;
}

var builder = WebApplication.CreateBuilder(args);

// Define CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        Console.WriteLine("Configuring CORS policy...");
        builder.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Configure Kestrel to listen on a specific port
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.TryParse(Environment.GetEnvironmentVariable("SERVER_PORT"), out var port) 
        ? port 
        : 5111); // Replace 5000 with your desired port
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); 
    logging.AddDebug();   
});


builder.Services.AddControllers(options =>
    {
        options.ModelBinderProviders.Insert(0, new RequestContextModelBinderProvider());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new AttributesJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new RelationshipsJsonConverter());
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IEndecryptor, Endecryptor>();
builder.Services.AddSingleton<IEmailSender, EmailSender>(_
    => new EmailSender("",80,"",""));
builder.Services.AddSingleton<IConnectionProvider, ConnectionProvider>(services => 
    new (GetConnectionString(services)));
builder.Services.AddSingleton<IEncryptionSecretProvider, EncryptionSecretProvider>();
builder.Services.AddRepositories();
builder.Services.AddSwaggerGen();
builder.Services.AddApiServices();

// var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
// var redisPort = int.TryParse(Environment.GetEnvironmentVariable("REDIS_PORT"), out var port) ? port : 6379;
// var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
//
// builder.Services.AddThrottler(string.IsNullOrEmpty(redisHost) 
//     ? new DefaultCachingStrategyConfiguration() 
//     : new RedisCachingStrategyConfiguration(
//         new DefaultRatelimitingConfiguration(), 
//         new ServerConfiguration(redisHost,redisPassword,redisPort)
//         )
//     );

var app = builder.Build();

// Apply CORS early with logging
app.UseCors(builder =>
{
    Console.WriteLine("CORS middleware executing for request...");
    builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

// Enable Swagger in All Environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniFlexCRM API v1");
    c.RoutePrefix = "swagger"; // Ensure the UI is available at `/swagger`
});

app.UseWhen(context => !(context.Request.Path.StartsWithSegments("/api/auth")
                         || context.Request.Path.StartsWithSegments("/swagger")),
    appBuilder =>
    {
        appBuilder.UseMiddleware<JwtAuthorizationMiddleware>();
    });

// app.UseMiddleware<RateLimitMiddleware>();

// Add CORS before routing and controllers

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
