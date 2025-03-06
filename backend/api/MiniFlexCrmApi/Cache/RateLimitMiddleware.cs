using MiniFlexCrmApi.Context;

namespace MiniFlexCrmApi.Cache;

public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IThrottlerCounter _counter;
    private readonly ICachingStrategyConfiguration _config;

    public RateLimitMiddleware(RequestDelegate next, IThrottlerCounter counter, ICachingStrategyConfiguration config)
    {
        _next = next;
        _counter = counter;
        _config = config;
    }

    public async Task Invoke(HttpContext context)
    {
        var requestContext = RequestContextExtractor.Extract(context);
        if (await _counter.ThunkAsync(requestContext).ConfigureAwait(false))
        {
            await _next(context).ConfigureAwait(false);
            return;
        }
        context.Response.StatusCode = 429;
        context.Response.ContentType = "application/json";
        context.Response.Headers["Retry-After"] 
            = $"{(int)(_config.RateLimitingConfiguration.WindowDuration.TotalSeconds*1.13)}";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            message = "You have exceeded the maximum number of requests. Please try again later."
        });
    }
}