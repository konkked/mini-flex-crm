using MiniFlexCrmApi.Context;
using MiniFlexCrmApi.Models;

public class RequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public RequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, RequestContext requestContext)
    {
        // Extract values from the request (e.g., headers, query parameters)
        var requestContextData = RequestContextExtractor.Extract(context);
        requestContext.TenantId = requestContextData.TenantId;
        requestContext.UserId = requestContextData.UserId;
        requestContext.Claims = requestContextData.Claims;
        requestContext.Headers = requestContextData.Headers;
        requestContext.Path = requestContextData.Path;
        requestContext.Username = requestContextData.Username;
        requestContext.QueryString = requestContextData.QueryString;
        requestContext.Role = requestContextData.Role;
        requestContext.Cookies = requestContextData.Cookies;

        await _next(context);
    }
}