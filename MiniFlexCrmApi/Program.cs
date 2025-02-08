using Microsoft.AspNetCore.Diagnostics;
using MiniFlexCrmApi.Api.Middleware;
using MiniFlexCrmApi.Api.Services;
using MiniFlexCrmApi.Db;
using MiniFlexCrmApi.Db.Repos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new RequestContextModelBinderProvider());
});

builder.Services.AddSingleton<IConnectionProvider, ConnectionProvider>();
builder.Services.AddRepositories();
builder.Services.AddSwaggerGen();
builder.Services.AddApiServices();

var app = builder.Build();

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/auth"), appBuilder =>
{
    appBuilder.UseMiddleware<JwtAuthorizationMiddleware>();
});


app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
