using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MiniFlexCrmApi.Api.Auth;
using MiniFlexCrmApi.Api.Context;
using MiniFlexCrmApi.Api.Serialization;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Db.Repos;

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

builder.Services.AddSingleton<IConnectionProvider, ConnectionProvider>(services => 
    new (GetConnectionString(services)));
builder.Services.AddSingleton<IJwtKeyProvider, JwtKeyProvider>();
builder.Services.AddRepositories();
builder.Services.AddSwaggerGen();
builder.Services.AddApiServices();

var app = builder.Build();

// Enable Swagger in All Environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniFlexCRM API v1");
    c.RoutePrefix = "swagger"; // Ensure the UI is available at `/swagger`
});

app.UseWhen(context => !(context.Request.Path.StartsWithSegments("/api/auth") 
                         || context.Request.Path.StartsWithSegments("/swagger")) , 
    appBuilder => 
    {
        appBuilder.UseMiddleware<JwtAuthorizationMiddleware>(); 
    });

app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
